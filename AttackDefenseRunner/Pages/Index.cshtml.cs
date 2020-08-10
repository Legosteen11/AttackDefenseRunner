using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace AttackDefenseRunner.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Tag { get; set; }
        
        public void OnGet()
        {
        }

        public void OnPost()
        {
            var tag = Tag;
            
            Log.Information("Tag is {tag}", tag);
        }
    }
}