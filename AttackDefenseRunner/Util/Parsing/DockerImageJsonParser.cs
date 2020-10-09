using System.IO;
using System.Threading.Tasks;
using AttackDefenseRunner.Util.Parsing.Json;

namespace AttackDefenseRunner.Util.Parsing
{
    public class DockerImageJsonParser
    {
        public Task<DockerTagJson> ParseDockerJsonTag(Stream body)
            => JsonUtil.FromStream<DockerTagJson>(body);
    }
}