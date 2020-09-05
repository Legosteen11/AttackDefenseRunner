using AttackDefenseRunner.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace AttackDefenseRunner.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ServiceManager _manager;

        public IndexModel(ServiceManager manager)
        {
            _manager = manager;
        }

        public void OnPostStartService()
        {
            _manager.StartService();
        }
        
        public void OnPostStopService()
        {
            _manager.StopService();
        }
    }
}