using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace AttackDefenseRunner.Util.Flag
{
    public class FlagFinderWorker : BackgroundService
    {
        private readonly IFlagFinder _finder;

        public FlagFinderWorker(IFlagFinder finder)
        {
            _finder = finder;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _finder.Start(s => s.Split(" "), stoppingToken);
            }
        }
    }
}