using Serilog;

namespace AttackDefenseRunner.Util
{
    public class RunningSingleton
    {
        public bool Running { get; set; }

        public void startService()
        {
            Log.Information("Starting Service");
            
            
            
            
            Running = true;
            Log.Information("Started Service");

        }

        public void stopService()
        {
            Log.Information("Stopping Service");
            
            
            
            Running = false;
            Log.Information("Stopped Service");

        }
        
        
    }
}