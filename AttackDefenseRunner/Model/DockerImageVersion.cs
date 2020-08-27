namespace AttackDefenseRunner.Model
{
    public class DockerImageVersion
    {
        public int Id { get; set; }
        
        public string Version { get; set; }
        public int DockerImageId { get; set; }
        
        public DockerImage DockerImage { get; set; }
    }
}