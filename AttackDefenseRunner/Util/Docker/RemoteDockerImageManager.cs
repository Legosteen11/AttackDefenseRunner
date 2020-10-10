using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AttackDefenseRunner.Model;
using AttackDefenseRunner.Util.Parsing;
using AttackDefenseRunner.Util.Parsing.Json;
using Microsoft.EntityFrameworkCore;
using Serilog;
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
        private readonly HttpClient _httpClient;
        private readonly List<IObserver<List<DockerContainer>>> _observers = new List<IObserver<ContainerList>>();

        public RemoteDockerImageManager(ADRContext context, DockerTagManager tagManager, IRemoteWorkerManager workManager, HttpClient httpClient)
        {
            _context = context;
            _tagManager = tagManager;
            _workManager = workManager;
            _httpClient = httpClient;
        }

        public async Task<DockerContainer> StartContainer(DockerTag tag)
        {
            await StopImage(tag.Tag);
            
            var worker = await NextWorker();

            var jsonString = $"{{\"Tag\"=\"{tag.Tag}\"}}";

            return await ApiPost<DockerContainer>(worker, Endpoint.UPDATE_IMAGE, jsonString);
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
            UsageJson totalUsage = new UsageJson
            {
                MemorySet = false,
                CpuSet = false
            };

            foreach (var usage in await GetUsages())
            {
                
            }

            return totalUsage;
        }

        private async Task<Dictionary<RemoteWorker, UsageJson>> GetUsages()
        {
            var usages = new Dictionary<RemoteWorker, UsageJson>();

            foreach (var worker in await _workManager.GetWorkers())
            {
                usages.Add(worker, await ApiGet<UsageJson>(worker, Endpoint.USAGE_BASE));
            }

            return usages;
        }

        private async Task<RemoteWorker> NextWorker()
            => (await GetUsages())
                .OrderByDescending(c => c.Value.MemoryLeft)
                .FirstOrDefault()
                .Key;

        private async Task<T> ApiGet<T>(RemoteWorker worker, string url)
        {
            var result = await _httpClient.GetAsync(worker.Host + url);

            var content = result.Content.ToString();

            return content == null ? default : JsonUtil.FromString<T>(content);
        }

        private async Task<T> ApiPost<T>(RemoteWorker worker, string url, string content)
        {
            var result = await _httpClient.PostAsync(worker.Host + url, new StringContent(content));

            content = result.Content.ToString();

            return content == null ? default : JsonUtil.FromString<T>(content);
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