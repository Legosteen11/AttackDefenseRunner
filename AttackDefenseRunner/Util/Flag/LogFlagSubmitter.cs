using Serilog;

namespace AttackDefenseRunner.Util.Flag
{
    public class LogFlagSubmitter : IFlagSubmitter
    {
        public void Submit(string flag)
        {
            if (flag == "") 
                return;
            
            Log.Information("Found flag: {flag}", flag);
        }
    }
}