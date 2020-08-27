namespace AttackDefenseRunner.Model
{
    public class DockerContainer
    {
        public int Id { get; set; }
        
        public int DockerImageVersionId { get; set; }

        public DockerImageVersion DockerImageVersion { get; set; }
    }
}