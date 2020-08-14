namespace AttackDefenseRunner.Model
{
    public class DockerContainer
    {
        public int Id { get; set; }
        
        public int DockerImageId { get; set; }

        public DockerImage DockerImage { get; set; }
    }
}