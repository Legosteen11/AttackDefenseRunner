using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace AttackDefenseRunner.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string DockerTag { get; set; }
        
        [BindProperty]
        public string FlagRegex { get; set; }
        
        [BindProperty]
        public string VulnServers { get; set; }
        
        [BindProperty]
        public string AttackServers { get; set; }
        public void OnGet()
        {
        }

        public void OnPost()
        {
            var dockertag = DockerTag;
            var flagregex = FlagRegex;
            var vulnservers = VulnServers;
            var attackservers = AttackServers;
            
            Log.Information("Docker Tag is {dockertag}", dockertag);
            Log.Information("Flag regex is {flagregex}", flagregex);
            Log.Information("Vulnerable servers are {vulnservers}", vulnservers);
            Log.Information("Attacking servers are {attackservers}", attackservers);

        }
    }
}