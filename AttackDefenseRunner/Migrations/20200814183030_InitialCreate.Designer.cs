﻿// <auto-generated />
using AttackDefenseRunner.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AttackDefenseRunner.Migrations
{
    [DbContext(typeof(ADRContext))]
    [Migration("20200814183030_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.7");

            modelBuilder.Entity("AttackDefenseRunner.Model.DockerContainer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("DockerImageId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("DockerImageId");

                    b.ToTable("DockerContainers");
                });

            modelBuilder.Entity("AttackDefenseRunner.Model.DockerImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Tag")
                        .HasColumnType("TEXT");

                    b.Property<string>("Version")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("DockerImages");
                });

            modelBuilder.Entity("AttackDefenseRunner.Model.GlobalSetting", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Key");

                    b.ToTable("GlobalSettings");
                });

            modelBuilder.Entity("AttackDefenseRunner.Model.DockerContainer", b =>
                {
                    b.HasOne("AttackDefenseRunner.Model.DockerImage", "DockerImage")
                        .WithMany()
                        .HasForeignKey("DockerImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
