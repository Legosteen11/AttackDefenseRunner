using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AttackDefenseRunner.Model;
using AttackDefenseRunner.Util.Docker;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AttackDefenseRunner.Util.Flag
{
    public class DockerFlagFinder : IFlagFinder, IObserver<List<DockerContainer>>
    {
        private readonly DockerClient _dockerClient;
        private readonly IFlagSubmitter _flagSubmitter;
        private readonly IServiceProvider _serviceProvider;
        private readonly RunningSingleton _running;
        private CancellationToken _cancellationToken;
        private bool _streamUpdate;
        private readonly SemaphoreSlim _streamLock = new SemaphoreSlim(1, 1);

        private Dictionary<DockerContainer, DateTimeOffset> streams = new Dictionary<DockerContainer, DateTimeOffset>();

        public DockerFlagFinder(IFlagSubmitter flagSubmitter, IServiceProvider serviceProvider, IDockerContainerObserver containerObserver, RunningSingleton running)
        {
            _flagSubmitter = flagSubmitter;
            _serviceProvider = serviceProvider;
            _running = running;
            _dockerClient = new DockerClientConfiguration()
                .CreateClient();
            containerObserver.Subscribe(this);
        }

        public async Task Start(IFlagFinder.FlagDelegate finder, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            
            ICollection<DockerContainer> dockerContainers;
            
            // TODO: We should automatically update this
            using (var scope = _serviceProvider.CreateScope())
            {
                ADRContext context = scope.ServiceProvider.GetRequiredService<ADRContext>();

                dockerContainers = await context.DockerContainers.ToListAsync(cancellationToken);
            }

            await UpdateStreams(dockerContainers);

            while (!cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(5000);

                // Request lock
                await _streamLock.WaitAsync(cancellationToken);
                
                // We have to convert this to an immutable list because else the runtime will complain about changed
                // dictionaries while enumerating
                foreach (var container in streams.Keys.ToImmutableList())
                {
                    // Check if streams haven't just been updated
                    if (_streamUpdate || cancellationToken.IsCancellationRequested)
                        break;
                    
                    var streamStop = DateTimeOffset.Now;
                    var streamStart = streams[container];

                    var stream = await GetStream(container, streamStart, streamStop);
                    
                    var output = await stream.ReadOutputToEndAsync(CancellationToken.None);

                    stream.Dispose();
                    
                    // Update streams time
                    streams[container] = streamStop;

                    // Replace some weird output from Docker logs
                    var lines = output.stdout.Replace("\u0001\u0000\u0000\u0000\u0000\u0000\u0000*", "").Split("\n");

                    foreach (var flag in lines.SelectMany(line => finder(line)))
                    {
                        if (_running.Running)
                        {
                            _flagSubmitter.Submit(flag);
                        }
                    }
                
                    Log.Information("Output of {container}: {@output}", container.DockerId, lines);
                }

                // Release lock
                _streamLock.Release();
            }
        }

        private async Task<MultiplexedStream> GetStream(DockerContainer container, DateTimeOffset from, DateTimeOffset until)
            => await _dockerClient.Containers.GetContainerLogsAsync(container.DockerId, true, new ContainerLogsParameters
            {
                ShowStderr = true,
                ShowStdout = true,
                Since = from.ToUnixTimeSeconds().ToString()
            }, _cancellationToken);

        private async Task UpdateStreams(ICollection<DockerContainer> containers)
        {
            // Notify process of updated streams
            _streamUpdate = true;
            
            // Lock streams
            await _streamLock.WaitAsync(_cancellationToken);
            
            // Get new streams of containers
            foreach (var container in containers)
            {
                if (!streams.ContainsKey(container))
                {
                    Log.Information("Listening to new container {containerId}", container.DockerId);
                    streams.Add(container, DateTimeOffset.Now);
                }
            }
            
            // Remove streams of removed containers
            foreach (var (container, _) in streams)
            {
                if (containers.Contains(container)) continue;

                // Stream should be removed
                Log.Information("Removing stream to container {containerId}", container.DockerId);
                streams.Remove(container);
            }

            _streamLock.Release();

            _streamUpdate = false;
        }
        
        public void OnCompleted()
        {
            // Do nothing.
        }

        public void OnError(Exception error)
        {
            // Do nothing
        }

        public async void OnNext(List<DockerContainer> value)
        {
            // Update list of containers
            await UpdateStreams(value);
        }
    }
}