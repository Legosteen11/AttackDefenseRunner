namespace AttackDefenseRunner.Model
{
    public class DockerTag
    {
        public int Id { get; set; }
        
        public string Version { get; set; }
        public string Tag { get; set; }
        public int DockerImageId { get; set; }
        
        public DockerImage DockerImage { get; set; }
    }
}