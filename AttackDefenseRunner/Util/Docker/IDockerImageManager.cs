using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AttackDefenseRunner.Model;

namespace AttackDefenseRunner.Util.Docker
{
    public interface IDockerImageManager : IObservable<List<DockerContainer>>

    {
    public Task<DockerContainer> StartContainer(DockerTag tag);

    public Task StopContainer(string id);

    public Task UpdateImage(string tagString);

    public Task StopImage(string tag);
    }
}