using System.Collections.Generic;
using System.Linq;
using AttackDefenseRunner.Model;
using Serilog;


namespace AttackDefenseRunner.Util
{
    public class DockerTagHandler
    {
        private readonly ADRContext _context;

        public DockerTagHandler(ADRContext context)
        {
            _context = context;
        }

        public void AddDockerTag(string dockertag)
        {
            
            Log.Information("Docker tag is {dockertag}", dockertag);
        }

        private List<DockerContainer> FetchTagsFromDatabase()
        {
            return _context.DockerContainers.ToList();
        }
        
    }
}