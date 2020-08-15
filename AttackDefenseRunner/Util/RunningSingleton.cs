using System.Threading;
using AttackDefenseRunner.Hubs;
using Serilog;

namespace AttackDefenseRunner.Util
{
    public class RunningSingleton
    {
        public bool Running { get; set; }
        
    }
}