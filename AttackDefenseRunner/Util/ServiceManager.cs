using System.Threading.Tasks;
using AttackDefenseRunner.Hubs;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace AttackDefenseRunner.Util
{
    public class ServiceManager
    {

        private readonly IHubContext<MonitorHub> _monitor;
        private readonly RunningSingleton _running;

        public ServiceManager(IHubContext<MonitorHub> monitor, RunningSingleton running)
        {
            _monitor = monitor;
            _running = running;
        }

        public async void StartService()
        {
            Log.Information("Starting Service");
            bool success = true;
            //TODO: actually start service


            
            if (success)
            {
                await _monitor.Clients.All.SendAsync("ServiceRunner", true);
                _running.Running = true;
                Log.Information("Started Service"); 
            }
        }

        public async void StopService()
        {
            Log.Information("Stopping Service");
            bool success = true;
            //TODO: actually stop service

            
            
            if (success)
            {
                await _monitor.Clients.All.SendAsync("ServiceRunner", false);
                _running.Running = false;
                Log.Information("Stopped Service");         
            }
        }
    }
}