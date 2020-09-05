using Microsoft.EntityFrameworkCore;

namespace AttackDefenseRunner.Model
{
    public class ADRContext : DbContext
    {
        public ADRContext(DbContextOptions<ADRContext> options) : base(options)
        {
        }

        public DbSet<GlobalSetting> GlobalSettings { get; set; }
        public DbSet<DockerImage> DockerImages { get; set; }
        public DbSet<DockerContainer> DockerContainers { get; set; }
        public DbSet<DockerTag> DockerTags { get; set; }
    }
}