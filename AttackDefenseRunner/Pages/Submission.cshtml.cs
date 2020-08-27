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
        public string DockerName { get; set; }
        
        [BindProperty]
        public string DockerTag { get; set; }
        
        public void OnGet()
        {
        }

        public void OnPost()
        {
            var dockername = DockerName;
            var dockertag = DockerTag;

            _dockertags.AddDockerTag(dockername, dockertag);

        }
    }
}