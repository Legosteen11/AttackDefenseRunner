using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AttackDefenseRunner.Model;
using AttackDefenseRunner.Util.Parsing.Json;
using AttackDefenseRunner.Util.Config;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using ContainerList = System.Collections.Generic.List<AttackDefenseRunner.Model.DockerContainer>;

namespace AttackDefenseRunner.Util.Docker
{
    public class LocalDockerImageManager : IDockerImageManager
    {
        private readonly ADRContext _context;
        private readonly DockerTagManager _tagManager;
        private readonly DockerClient _dockerClient;
        private readonly FileStrings _files;
        private readonly SettingsHelper _settings;
        private readonly List<IObserver<List<DockerContainer>>> _observers = new List<IObserver<ContainerList>>();

        public LocalDockerImageManager(ADRContext context, DockerTagManager tagManager, IDockerContainerObserver containerObserver, IOptions<FileStrings> files, SettingsHelper settings)
        {
            _context = context;
            _tagManager = tagManager;
            _files = files.Value;
            _settings = settings;
            containerObserver.Subscribe(this);
            // TODO: Create something nicer for this so we can add settings etc.
            _dockerClient = new DockerClientConfiguration()
                .CreateClient();
        }

        public async Task<DockerContainer> StartContainer(DockerTag tag)
        {
            string containerName = TagHelper.CreateName(tag.Tag, tag.DockerImageId);
            
            // Start tag
            // Pull the image
            Log.Information("Pulling image {image}", tag.Tag);
            // TODO: Fix this with some API call because this is not secure :-).
            $"docker pull {tag.Tag}".Bash();
            
            // First create the image:
            var container = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = tag.Tag,
                Name = containerName,
                HostConfig = new HostConfig
                {
                    RestartPolicy = new RestartPolicy
                    {
                        Name = RestartPolicyKind.Always
                    },
                    Binds = new List<string>
                    {
                        _files.TargetFileLocation + ":" + _files.TargetFileLocationDocker
                    }
                }
            });

            await _dockerClient.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());
            
            Log.Information("Started container {name}, {id} for {tag}", containerName, container.ID, tag.Tag);
            
            DockerContainer dockerContainer = new DockerContainer
            {
                DockerTag = tag,
                DockerId = container.ID
            };

            await _context.AddAsync(dockerContainer);
            await _context.SaveChangesAsync();

            await Notify();

            return dockerContainer;
        }

        public async Task StopContainer(string id)
        {
            await _dockerClient.Containers.StopContainerAsync(id, new ContainerStopParameters());
            try
            {
                await _dockerClient.Containers.RemoveContainerAsync(id, new ContainerRemoveParameters());
            }
            catch (Exception)
            {
                // Ignore exception
            }

            // Remove any containers from the database with this id
            _context.RemoveRange(await _context.DockerContainers.Where(dockerContainer => dockerContainer.DockerId == id).ToListAsync());
            await _context.SaveChangesAsync();

            await Notify();
        }

        public async Task<DockerContainer> UpdateImage(string tagString)
        {
            // Get or create the tag
            DockerTag tag = await _tagManager.GetOrCreateTag(tagString);

            // Check if there are any running instances of this image
            var runningIds = await GetContainers(TagHelper.GetImage(tagString), TagHelper.CreateImageOnlyName(tagString, tag.DockerImageId));
            
            // Update targets
            await UpdateTargets();

            bool alreadyRunning = false;
            DockerContainer runningContainer = null;
            
            foreach (var container in runningIds)
            {
                if (container.Image != tagString)
                {
                    Log.Information("Stopping container {id}", container.ID);
                    await StopContainer(container.ID);
                }
                else 
                {
                    alreadyRunning = true;
                    runningContainer = await _context
                        .DockerContainers
                        .Where(c => c.DockerId == container.ID)
                        .FirstAsync();
                }
            }

            if (!alreadyRunning)
            {
                runningContainer = await StartContainer(tag);
            }

            return runningContainer;
        }

        public async Task StopImage(string tagString)
        {
            DockerImage image = await _tagManager.GetImage(tagString);
            
            if (image == null)
                throw new ImageNotFoundException(tagString);

            var runningIds = await GetContainers(TagHelper.GetImage(tagString), TagHelper.CreateImageOnlyName(tagString, image.Id));

            string imageString = TagHelper.GetImage(tagString);
            
            foreach (var container in runningIds)
            {
                // Somehow we found an incorrect match we should ignore
                if (container.Image.Split(":")[0] != imageString) 
                    continue;
                
                Log.Information("Stopping container {id}", container.ID);
                await StopContainer(container.ID);
            }

            await Notify();
        }

        public async Task UpdateTargets(ICollection<string> targets)
        {
            Log.Information("Updating targets to {@targets}", targets);

            Directory.CreateDirectory(_files.TargetFileDir);
            
            await using var file = File.CreateText(_files.TargetFileLocation);
            
            await file.WriteAsync(string.Join("\n", targets));
        }
        
        private async Task UpdateTargets()
            => await UpdateTargets(_settings.GetValue(SettingsHelper.VULNSERVERS_KEY).Split("\n"));

        public async Task<UsageJson> GetUsage()
        {
            // Output looks something like this:
            //     total        used        free      shared  buff/cache   available
            // Mem:          11673        6657         238         461        4778        4241
            // Swap:          5903          31        5872
            var memory = "free -m"
                .Bash()
                .Split("\n")[1]
                .Split(" ", StringSplitOptions.RemoveEmptyEntries);

            var totalMem = ulong.Parse(memory[1]);
            var usedMem = ulong.Parse(memory[2]);
            
            UsageJson usage = new UsageJson
            {
                MemorySet = true,
                MemoryTotalAvailable = totalMem,
                MemoryUsage = usedMem,
                MemoryLeft = totalMem - usedMem
            };
            
            var testContainer = await _context.DockerContainers.FirstOrDefaultAsync();

            if (testContainer == null)
                return usage;

            // Create a semaphore to make us wait for the container stats response to finish
            SemaphoreSlim s = new SemaphoreSlim(0, 1);
            
            await _dockerClient
                .Containers
                .GetContainerStatsAsync(testContainer.DockerId, new ContainerStatsParameters
                    {
                        Stream = false
                    },
                    new Progress<ContainerStatsResponse>(r =>
                    {
                        var memory = r.MemoryStats;
                        var cpu = r.CPUStats;
                        var prevCpu = r.PreCPUStats;
            
                        var cpuDelta = cpu.CPUUsage.TotalUsage - prevCpu.CPUUsage.TotalUsage;
                        var systemCpuDelta = cpu.SystemUsage - prevCpu.SystemUsage;
                        var maxCpuUsage = cpu.OnlineCPUs * 100;
                        var cpuUsage = (cpuDelta / systemCpuDelta) * maxCpuUsage;
            
                        var usedMemory = memory.Usage - memory.Stats["cache"];
                        var maxMemory = memory.Limit;

                        usage.CpuSet = true;
                        usage.CpuUsage = cpuUsage;
                        usage.CpuTotalAvailable = maxCpuUsage;
                        usage.CpuLeft = maxCpuUsage - cpuUsage;

                        s.Release();
                    }));
            
            // Wait for the request to finish
            await s.WaitAsync();
            
            return usage;
        }

        private async Task<ICollection<ContainerListResponse>> GetContainers(string imageString, string nameString)
            => (await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters
            {
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    // {"label", new Dictionary<string, bool> {{imageString, true}}}
                },
                All = true
            })).Where(container => TagHelper.GetImage(container.Image) == imageString).ToList();

        private async Task Notify()
        {
            var containerList = await _context.DockerContainers.ToListAsync(); 
            
            foreach (var observer in _observers)
            {
                observer.OnNext(containerList);
            }
        }
        
        public IDisposable Subscribe(IObserver<List<DockerContainer>> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            return new Unsubscriber(_observers, observer);
        }
    }
}