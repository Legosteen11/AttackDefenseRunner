using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace AttackDefenseRunner.Pages
{
    public class SubmissionModel : PageModel
    {
        [BindProperty]
        public string DockerTag { get; set; }
        
        public void OnGet()
        {
        }

        public void OnPost()
        {
            var dockertag = DockerTag;
            Log.Information("Docker Tag is {dockertag}", dockertag);

        }
    }
}