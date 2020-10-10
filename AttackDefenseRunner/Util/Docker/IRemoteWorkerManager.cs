using System.Collections.Generic;
using System.Threading.Tasks;

namespace AttackDefenseRunner.Util.Docker
{
    public interface IRemoteWorkerManager
    {
        public Task RemoveWorker(RemoteWorker worker);
        public Task AddWorker(RemoteWorker worker);
        public Task<ICollection<RemoteWorker>> GetWorkers();
    }
}