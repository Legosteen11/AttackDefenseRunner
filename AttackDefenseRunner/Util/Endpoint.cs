namespace AttackDefenseRunner.Util
{
    public class Endpoint
    {
        public const string API_BASE = "/api";
        public const string ID = "/{id}";
        public const string USAGE_BASE = API_BASE + "/usage";
        public const string CONTAINER_BASE = API_BASE + "/container";
        public const string IMAGE_BASE = API_BASE + "/image";
        public const string UPDATE_IMAGE = IMAGE_BASE + "/update";
        public const string STOP = "/stop";
        public const string TARGETS_BASE = API_BASE + "/target";
    }
}