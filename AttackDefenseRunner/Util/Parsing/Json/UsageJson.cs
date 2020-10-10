namespace AttackDefenseRunner.Util.Parsing.Json
{
    public class UsageJson
    {
        public bool CpuSet { get; set; }
        public ulong CpuUsage { get; set; }
        public ulong CpuLeft { get; set; }
        public ulong CpuTotalAvailable { get; set; }
        public bool MemorySet { get; set; }
        public ulong MemoryUsage { get; set; }
        public ulong MemoryLeft { get; set; }
        public ulong MemoryTotalAvailable { get; set; }
    }
}