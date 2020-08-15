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
            _running.startService();
        }
        
        public void OnPostStopService()
        {
            _running.stopService();
        }
    }
}