namespace AttackDefenseRunner.Model
{
    public class DockerContainer
    {
        public int Id { get; set; }
        
        public string DockerId { get; set; }
        
        public int DockerTagId { get; set; }

        public DockerTag DockerTag { get; set; }
    }
}