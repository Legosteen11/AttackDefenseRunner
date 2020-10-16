using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AttackDefenseRunner.Model;
using AttackDefenseRunner.Util.Parsing;
using AttackDefenseRunner.Util.Parsing.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using ContainerList = System.Collections.Generic.List<AttackDefenseRunner.Model.DockerContainer>;

namespace AttackDefenseRunner.Util.Docker
{
    /// <summary>
    /// This Docker image manager lets you manage images on remote ADR servers
    /// </summary>
    public class RemoteDockerImageManager : IDockerImageManager
    {
        private readonly ADRContext _context;
        private readonly DockerTagManager _tagManager;
        private readonly IRemoteWorkerManager _workManager;
        private readonly HttpClient _httpClient;
        private readonly SettingsHelper _settings;
        private readonly List<IObserver<List<DockerContainer>>> _observers = new List<IObserver<ContainerList>>();

        public RemoteDockerImageManager(ADRContext context, DockerTagManager tagManager, IRemoteWorkerManager workManager, HttpClient httpClient, SettingsHelper settings)
        {
            _context = context;
            _tagManager = tagManager;
            _workManager = workManager;
            _httpClient = httpClient;
            _settings = settings;
        }

        public async Task<DockerContainer> StartContainer(DockerTag tag)
        {
            await StopImage(tag.Tag);
            
            var worker = await NextWorker();

            var jsonString = $"{{\"Tag\"=\"{tag.Tag}\"}}";

            var returnedContainer = await ApiPost<DockerContainer>(worker, Endpoint.UPDATE_IMAGE, jsonString);
            
            DockerContainer dockerContainer = new DockerContainer
            {
                DockerTag = tag,
                DockerId = returnedContainer.DockerId
            };

            await _context.AddAsync(dockerContainer);
            await _context.SaveChangesAsync();

            await Notify();

            return dockerContainer;
        }

        public async Task StopContainer(string id)
        {
            Log.Information("Stopping container {id}", id);
            
            var workers = await _workManager.GetWorkers();

            foreach (var worker in workers)
            {
                await ApiPost<string>(worker, Endpoint.CONTAINER_BASE + $"/{id}" + Endpoint.STOP, string.Empty);
            }
        }

        public async Task<DockerContainer> UpdateImage(string tagString)
        {
            Log.Information("Updating image {tag}", tagString);
            
            // Stop all instances of this image
            await StopImage(tagString);
            
            // Get or create the tag
            DockerTag tag = await _tagManager.GetOrCreateTag(tagString);

            // Update targets
            await UpdateTargets();

            // Start the container
            DockerContainer runningContainer = await StartContainer(tag);

            return runningContainer;
        }

        public async Task StopImage(string tag)
        {
            Log.Information("Stopping image {tag}", tag);
            
            var workers = await _workManager.GetWorkers();

            foreach (var worker in workers)
            {
                await ApiPost<string>(worker, Endpoint.IMAGE_BASE + $"/{tag}" + Endpoint.STOP, string.Empty);
            }
        }

        public async Task<UsageJson> GetUsage()
        {
            UsageJson totalUsage = new UsageJson
            {
                MemorySet = false,
                CpuSet = false
            };

            foreach (var (worker, usage) in await GetUsages())
            {
                if (usage.CpuSet)
                {
                    totalUsage.CpuSet = true;
                    totalUsage.CpuLeft += usage.CpuLeft;
                    totalUsage.CpuUsage += usage.CpuUsage;
                    totalUsage.CpuTotalAvailable += usage.CpuTotalAvailable;
                }

                if (usage.MemorySet)
                {
                    totalUsage.MemorySet = true;
                    totalUsage.MemoryLeft += usage.MemoryLeft;
                    totalUsage.MemoryUsage += usage.MemoryUsage;
                    totalUsage.MemoryTotalAvailable += usage.MemoryTotalAvailable;
                }
            }

            return totalUsage;
        }

        public async Task UpdateTargets(ICollection<string> targets)
        {
            foreach (var worker in await _workManager.GetWorkers())
            {
                await ApiPost<string>(worker, Endpoint.TARGETS_BASE, string.Join("\n", targets));
            }
        }
        
        private async Task UpdateTargets()
            => await UpdateTargets(_settings.GetValue(SettingsHelper.VULNSERVERS_KEY).Split("\n"));

        private async Task<Dictionary<RemoteWorker, UsageJson>> GetUsages()
        {
            var usages = new Dictionary<RemoteWorker, UsageJson>();

            foreach (var worker in await _workManager.GetWorkers())
            {
                usages.Add(worker, await ApiGet<UsageJson>(worker, Endpoint.USAGE_BASE));
            }

            return usages;
        }

        private async Task<RemoteWorker> NextWorker()
            => (await GetUsages())
                .OrderByDescending(c => c.Value.MemoryLeft)
                .FirstOrDefault()
                .Key;

        private async Task<T> ApiGet<T>(RemoteWorker worker, string url)
        {
            var result = await _httpClient.GetAsync(worker.Host + url);

            var content = result.Content.ToString();

            return content == null ? default : JsonUtil.FromString<T>(content);
        }

        private async Task<T> ApiPost<T>(RemoteWorker worker, string url, string content)
        {
            var result = await _httpClient.PostAsync(worker.Host + url, new StringContent(content));

            content = result.Content.ToString();

            return string.IsNullOrEmpty(content) ? default : JsonUtil.FromString<T>(content);
        }

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