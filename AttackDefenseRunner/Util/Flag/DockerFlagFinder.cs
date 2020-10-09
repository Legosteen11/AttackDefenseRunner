using System;
using System.Collections.Generic;
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
        private CancellationToken _cancellationToken;
        private bool _streamUpdate;
        private readonly SemaphoreSlim _streamLock = new SemaphoreSlim(1, 1);

        private Dictionary<DockerContainer, MultiplexedStream> streams = new Dictionary<DockerContainer, MultiplexedStream>();

        public DockerFlagFinder(IFlagSubmitter flagSubmitter, IServiceProvider serviceProvider)
        {
            _flagSubmitter = flagSubmitter;
            _serviceProvider = serviceProvider;
            _dockerClient = new DockerClientConfiguration()
                .CreateClient();
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
                
                foreach (var stream in streams.Values)
                {
                    // Check if streams haven't just been updated
                    if (_streamUpdate)
                        break;
                    
                    var output = await stream.ReadOutputToEndAsync(CancellationToken.None);

                    foreach (var flag in finder(output.stdout))
                    {
                        _flagSubmitter.Submit(flag);
                    }
                
                    Log.Information("Output: {@output}", output);
                }

                // Release lock
                _streamLock.Release();
            }
        }

        private async Task<MultiplexedStream> GetStream(DockerContainer container)
            => await _dockerClient.Containers.GetContainerLogsAsync(container.DockerId, true, new ContainerLogsParameters
            {
                ShowStderr = true,
                ShowStdout = true
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
                    streams.Add(container, await GetStream(container));
                }
            }
            
            // Remove streams of removed containers
            foreach (var (container, stream) in streams)
            {
                if (containers.Contains(container)) continue;

                // Stream should be removed
                Log.Information("Removing stream to container {containerId}", container.DockerId);
                stream.Dispose();
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