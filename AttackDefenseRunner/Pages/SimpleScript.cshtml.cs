using System.Threading.Tasks;
using AttackDefenseRunner.Util;
using AttackDefenseRunner.Util.Docker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
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
        public string Name { get; set; }
        [BindProperty]
        public string Requirements { get; set; }

        [BindProperty]
        public string PythonCode { get; set; }

        public void OnPostSimpleScript()
        {
            var name = Name;
            var requirements = Requirements;
            var pythoncode = PythonCode;

            Log.Information("A user submitted a simple script with the following values:");
            Log.Information($"Name: {name}", name);
            Log.Information($"Requirements: {requirements}",requirements);
            Log.Information($"Python code: {pythoncode}", pythoncode);
            
            foreach (var req in requirements.Split(';'))
            {
                //TODO add each element as a line in requirements.txt
            }
            
            
        }
    }
}