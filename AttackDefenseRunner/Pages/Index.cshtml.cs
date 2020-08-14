using AttackDefenseRunner.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace AttackDefenseRunner.Pages
{
    public class IndexModel : PageModel
    {
        private RunningSingleton _running { get; }

        public IndexModel(RunningSingleton running)
        {
            _running = running;
        }

        public void OnPostStartService()
        {
            Log.Information("Started Service");
            _running.Running = true;
        }
        
        public void OnPostStopService()
        {
            Log.Information("Stopped Service");
            _running.Running = false;
        }
    }
}