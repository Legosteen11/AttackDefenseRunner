﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace AttackDefenseRunner.Migrations
{
    public partial class ChangeDockerImageVersion : Migration
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
                name: "Tag",
                table: "DockerImages");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "DockerImages");

            migrationBuilder.DropColumn(
                name: "DockerImageId",
                table: "DockerContainers");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "DockerImages",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DockerTagId",
                table: "DockerContainers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DockerTags",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Version = table.Column<string>(nullable: true),
                    Tag = table.Column<string>(nullable: true),
                    DockerImageId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DockerTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DockerTags_DockerImages_DockerImageId",
                        column: x => x.DockerImageId,
                        principalTable: "DockerImages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DockerContainers_DockerTagId",
                table: "DockerContainers",
                column: "DockerTagId");

            migrationBuilder.CreateIndex(
                name: "IX_DockerTags_DockerImageId",
                table: "DockerTags",
                column: "DockerImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_DockerContainers_DockerTags_DockerTagId",
                table: "DockerContainers",
                column: "DockerTagId",
                principalTable: "DockerTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DockerContainers_DockerTags_DockerTagId",
                table: "DockerContainers");

            migrationBuilder.DropTable(
                name: "DockerTags");

            migrationBuilder.DropIndex(
                name: "IX_DockerContainers_DockerTagId",
                table: "DockerContainers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "DockerImages");

            migrationBuilder.DropColumn(
                name: "DockerTagId",
                table: "DockerContainers");

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "DockerImages",
                type: "TEXT",
                nullable: true);

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