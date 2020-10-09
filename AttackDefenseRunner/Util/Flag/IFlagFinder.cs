using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AttackDefenseRunner.Util.Flag
{
    public interface IFlagFinder
    {
        delegate ICollection<string> FlagDelegate(string s);
        
        public Task Start(FlagDelegate finder, CancellationToken cancellationToken);
    }
}