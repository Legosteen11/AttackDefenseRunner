using AttackDefenseRunner.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace AttackDefenseRunner.Pages
{
    public class SubmissionModel : PageModel
    {
        private readonly DockerTagHandler _dockertags;

        public SubmissionModel(DockerTagHandler dockertags)
        {
            _dockertags = dockertags;
        }
        

        [BindProperty]
        public string DockerTag { get; set; }
        
        public void OnGet()
        {
        }

        public void OnPost()
        {
            var dockertag = DockerTag;

            _dockertags.AddDockerTag(dockertag);

        }
    }
}