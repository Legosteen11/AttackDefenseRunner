using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AttackDefenseRunner.Model;
using AttackDefenseRunner.Util.Parsing.Json;

namespace AttackDefenseRunner.Util.Docker
{
    public interface IDockerImageManager : IObservable<List<DockerContainer>>
    {
        public Task<DockerContainer> StartContainer(DockerTag tag);

        public Task StopContainer(string id);

        public Task<DockerContainer> UpdateImage(string tagString);

        public Task StopImage(string tag);

        public Task<UsageJson> GetUsage();

        public Task UpdateTargets(ICollection<string> targets);
    }
}