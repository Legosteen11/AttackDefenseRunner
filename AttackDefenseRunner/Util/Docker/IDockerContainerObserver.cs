using System;
using System.Collections.Generic;
using AttackDefenseRunner.Model;

namespace AttackDefenseRunner.Util.Docker
{
    public interface IDockerContainerObserver : IObserver<List<DockerContainer>>, IObservable<List<DockerContainer>>
    {
        public void Subscribe(IObservable<List<DockerContainer>> provider);
    }
}