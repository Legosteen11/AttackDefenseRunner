using System.Threading.Tasks;
using AttackDefenseRunner.Util;
using AttackDefenseRunner.Util.Docker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace AttackDefenseRunner.Pages
{
    public class SubmissionModel : PageModel
    {
        private readonly IDockerImageManager _dockerImageManager;

        public SubmissionModel(IDockerImageManager dockerImageManager)
        {
            _dockerImageManager = dockerImageManager;
        }
        

        [BindProperty]
        public string DockerTag { get; set; }
        
        public void OnGet()
        {
        }

        public async Task OnPost()
        {
            string tagString = DockerTag;

            await _dockerImageManager.UpdateImage(tagString);
        }
    }
}