using Microsoft.EntityFrameworkCore.Migrations;

namespace AttackDefenseRunner.Migrations
{
    public partial class AddDockerImageVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DockerContainers_DockerImages_DockerImageId",
                table: "DockerContainers");

            migrationBuilder.DropIndex(
                name: "IX_DockerContainers_DockerImageId",
                table: "DockerContainers");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "DockerImages");

            migrationBuilder.DropColumn(
                name: "DockerImageId",
                table: "DockerContainers");

            migrationBuilder.AddColumn<int>(
                name: "DockerImageVersionId",
                table: "DockerContainers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DockerImageVersion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Version = table.Column<string>(nullable: true),
                    DockerImageId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DockerImageVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DockerImageVersion_DockerImages_DockerImageId",
                        column: x => x.DockerImageId,
                        principalTable: "DockerImages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DockerContainers_DockerImageVersionId",
                table: "DockerContainers",
                column: "DockerImageVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_DockerImageVersion_DockerImageId",
                table: "DockerImageVersion",
                column: "DockerImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_DockerContainers_DockerImageVersion_DockerImageVersionId",
                table: "DockerContainers",
                column: "DockerImageVersionId",
                principalTable: "DockerImageVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DockerContainers_DockerImageVersion_DockerImageVersionId",
                table: "DockerContainers");

            migrationBuilder.DropTable(
                name: "DockerImageVersion");

            migrationBuilder.DropIndex(
                name: "IX_DockerContainers_DockerImageVersionId",
                table: "DockerContainers");

            migrationBuilder.DropColumn(
                name: "DockerImageVersionId",
                table: "DockerContainers");

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "DockerImages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DockerImageId",
                table: "DockerContainers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DockerContainers_DockerImageId",
                table: "DockerContainers",
                column: "DockerImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_DockerContainers_DockerImages_DockerImageId",
                table: "DockerContainers",
                column: "DockerImageId",
                principalTable: "DockerImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
