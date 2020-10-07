using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AttackDefenseRunner.Model;
using AttackDefenseRunner.Util.Docker;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AttackDefenseRunner.Util.Flag
{
    public class DockerFlagFinder : IFlagFinder
    {
        private readonly DockerClient _dockerClient;
        private readonly IFlagSubmitter _flagSubmitter;
        private readonly IServiceProvider _serviceProvider;

        private List<MultiplexedStream> streams = new List<MultiplexedStream>();

        public DockerFlagFinder(IFlagSubmitter flagSubmitter, IServiceProvider serviceProvider)
        {
            _flagSubmitter = flagSubmitter;
            _serviceProvider = serviceProvider;
            _dockerClient = new DockerClientConfiguration()
                .CreateClient();
        }

        public async Task Start(IFlagFinder.FlagDelegate finder, CancellationToken cancellationToken)
        {
            ICollection<DockerContainer> dockerContainers;
            
            // TODO: We should automatically update this
            using (var scope = _serviceProvider.CreateScope())
            {
                DockerTagManager tagManager = scope.ServiceProvider.GetRequiredService<DockerTagManager>();

                dockerContainers = await tagManager.GetContainers();
            }

            foreach (var container in dockerContainers)
            {
                streams.Add(await _dockerClient.Containers.GetContainerLogsAsync(container.DockerId, true, new ContainerLogsParameters
                {
                    ShowStderr = true,
                    ShowStdout = true
                }, cancellationToken));
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(5000);

                foreach (var stream in streams)
                {
                    var output = await stream.ReadOutputToEndAsync(CancellationToken.None);

                    foreach (var flag in finder(output.stdout))
                    {
                        _flagSubmitter.Submit(flag);
                    }
                
                    Log.Information("Output: {@output}", output);
                }
            }
        }
    }
}