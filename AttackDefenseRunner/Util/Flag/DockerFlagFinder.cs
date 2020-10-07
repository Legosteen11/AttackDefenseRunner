using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Serilog;

namespace AttackDefenseRunner.Util.Flag
{
    public class DockerFlagFinder : IFlagFinder
    {
        private readonly DockerClient _dockerClient;
        private readonly IFlagSubmitter _flagSubmitter;

        public DockerFlagFinder(IFlagSubmitter flagSubmitter)
        {
            _flagSubmitter = flagSubmitter;
            _dockerClient = new DockerClientConfiguration()
                .CreateClient();
        }

        public async Task Start(IFlagFinder.FlagDelegate finder, CancellationToken cancellationToken)
        {
            var stream = await _dockerClient.Containers.GetContainerLogsAsync("cdb7eedc7a2b", true, new ContainerLogsParameters
            {
                ShowStderr = true,
                ShowStdout = true
            }, cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(5000);

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