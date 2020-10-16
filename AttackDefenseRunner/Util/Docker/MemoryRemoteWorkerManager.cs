using System.Collections.Generic;
using System.Threading.Tasks;

namespace AttackDefenseRunner.Util.Docker
{
    /// <summary>
    /// Simple in-memory implementation of a remote work manager
    /// </summary>
    public class MemoryRemoteWorkerManager : IRemoteWorkerManager
    {
        private readonly ICollection<RemoteWorker> _remoteWorkers = new List<RemoteWorker>();
        
        public Task RemoveWorker(RemoteWorker worker)
        {
            _remoteWorkers.Remove(worker);

            return Task.CompletedTask;
        }

        public Task AddWorker(RemoteWorker worker)
        {
            _remoteWorkers.Add(worker);
            
            return Task.CompletedTask;
        }

        public Task<ICollection<RemoteWorker>> GetWorkers()
        {
            return Task.FromResult(_remoteWorkers);
        }
    }
}