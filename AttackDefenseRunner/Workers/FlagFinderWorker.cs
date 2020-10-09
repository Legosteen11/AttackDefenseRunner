using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;

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
                try
                {
                    await _finder.Start(s => Regex.Matches(s, @".* Test .*$").Select(str => str.Value).ToList(), stoppingToken);
                }
                catch (Exception e)
                {
                    Log.Error("Flag finder restart with exception {e}", e);
                }
            }
        }
    }
}