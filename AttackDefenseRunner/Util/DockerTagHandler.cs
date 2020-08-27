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

        public void AddDockerTag(string dockername, string dockertag)
        {

            
            Log.Information("Docker name is {dockername}", dockername);
            Log.Information("Docker tag is {dockertag}", dockertag);
        }
    }
}