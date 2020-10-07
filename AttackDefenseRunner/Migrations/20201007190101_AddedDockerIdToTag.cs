using Microsoft.EntityFrameworkCore.Migrations;

namespace AttackDefenseRunner.Migrations
{
    public partial class AddedDockerIdToTag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DockerId",
                table: "DockerContainers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DockerId",
                table: "DockerContainers");
        }
    }
}
