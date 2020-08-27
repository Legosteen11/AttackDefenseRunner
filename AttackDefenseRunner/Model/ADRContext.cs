using Microsoft.EntityFrameworkCore;

namespace AttackDefenseRunner.Model
{
    public class ADRContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=adr.sqlite3");
        
        public DbSet<GlobalSetting> GlobalSettings { get; set; }
        public DbSet<DockerImage> DockerImages { get; set; }
        public DbSet<DockerContainer> DockerContainers { get; set; }
        public DbSet<DockerTag> DockerTags { get; set; }
    }
}