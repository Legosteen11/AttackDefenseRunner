using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AttackDefenseRunner.Model;
using AttackDefenseRunner.Util.Parsing.Json;
using Microsoft.EntityFrameworkCore;
using ContainerList = System.Collections.Generic.List<AttackDefenseRunner.Model.DockerContainer>;

namespace AttackDefenseRunner.Util.Docker
{
    /// <summary>
    /// This Docker image manager lets you manage images on remote ADR servers
    /// </summary>
    public class RemoteDockerImageManager : IDockerImageManager
    {
        private readonly ADRContext _context;
        private readonly DockerTagManager _tagManager;
        private readonly IRemoteWorkerManager _workManager;
        private readonly List<IObserver<List<DockerContainer>>> _observers = new List<IObserver<ContainerList>>();

        public RemoteDockerImageManager(ADRContext context, DockerTagManager tagManager, IRemoteWorkerManager workManager)
        {
            _context = context;
            _tagManager = tagManager;
            _workManager = workManager;
        }

        public Task<DockerContainer> StartContainer(DockerTag tag)
        {
        }

        public Task StopContainer(string id)
        {
            throw new NotImplementedException();
        }

        public Task<DockerContainer> UpdateImage(string tagString)
        {
            throw new NotImplementedException();
        }

        public Task StopImage(string tag)
        {
            throw new NotImplementedException();
        }

        public async Task<UsageJson> GetUsage()
        {
            UsageJson usage = new UsageJson
            {
                MemorySet = false,
                CpuSet = false
            };

            foreach (var worker in await _workManager.GetWorkers())
            {
                // TODO: Get usage and add it up
            }

            return usage;
        }

        private Task<RemoteWorker> NextWorker()
        {
            
        }

        private async Task Notify()
        {
            var containerList = await _context.DockerContainers.ToListAsync(); 
            
            foreach (var observer in _observers)
            {
                observer.OnNext(containerList);
            }
        }
        
        public IDisposable Subscribe(IObserver<List<DockerContainer>> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            return new Unsubscriber(_observers, observer);
        }
    }
}