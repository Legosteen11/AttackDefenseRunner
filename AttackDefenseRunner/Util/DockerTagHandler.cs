using System.Collections.Generic;
using System.Linq;
using AttackDefenseRunner.Model;
using Serilog;
using System.Text.RegularExpressions;

namespace AttackDefenseRunner.Util
{
    public class DockerTagHandler
    {
        private readonly ADRContext _context;
        private const string DockerRegex = "^(?:(?=[^:\\/]{1,253})(?!-)[a-zA-Z0-9-]{1,63}(?<!-)(?:\\.(?!-)[a-zA-Z0-9-]{1,63}(?<!-))*(?::[0-9]{1,5})?/)?(?<name>(?![._-])(?:[a-z0-9._-]*)(?<![._-])(?:/(?![._-])[a-z0-9._-]*(?<![._-]))*):(?<version>(?![.-])[a-zA-Z0-9_.-]{1,128})?$";
        public DockerTagHandler(ADRContext context)
        {
            _context = context;
        }

        public void AddDockerTag(string InputDockerTag)
        {
            Match match = Regex.Match(InputDockerTag, DockerRegex);
            if (match.Success)
            {
                Log.Information("Docker tag matched!");
                string NewDockerName = match.Groups["name"].Value;
                string NewDockerVersion = match.Groups["version"].Value;
                string NewDockerTag = match.Value;
                Log.Information("Docker tag: {DockerTag}", NewDockerTag);
                Log.Information("Docker tag name: {DockerName}", NewDockerName);
                Log.Information("Docker tag version: {DockerVersion}", NewDockerVersion);

                
            }
            else
            {
                Log.Information("Docker tag did not match...");

            }
            
            // List<DockerContainer> dockerlist = this.FetchTagsFromDatabase();
            // Log.Information("Docker tag list: {dockerlist}",dockerlist);
        }

        private List<DockerContainer> FetchTagsFromDatabase()
        {
            return _context.DockerContainers.ToList();
        }
        
    }
}