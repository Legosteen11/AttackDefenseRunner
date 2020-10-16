using System.Threading.Tasks;
using AttackDefenseRunner.Util;
using AttackDefenseRunner.Util.Docker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace AttackDefenseRunner.Pages
{
    public class SimpleScript : PageModel
    {
        private readonly ServiceManager _manager;
        private readonly IDockerImageManager _imageManager;

        public SimpleScript(SettingsHelper settings, ServiceManager manager, IDockerImageManager imageManager)
        {
            _manager = manager;
            _imageManager = imageManager;
        }

        [BindProperty]
        public string name { get; set; }
        [BindProperty]
        public string requirements { get; set; }

        [BindProperty]
        public string pythoncode { get; set; }

        public async Task OnPostSubmit()
        {

        }
    }
}