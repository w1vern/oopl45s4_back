﻿// <auto-generated />
using System;
using MafiaAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MafiaAPI.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MafiaAPI.Models.Match", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime?>("MatchEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("MatchResult")
                        .HasColumnType("text");

                    b.Property<DateTime?>("MatchStart")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("currentState")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Matches", (string)null);
                });

            modelBuilder.Entity("MafiaAPI.Models.PlayerState", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<bool>("IsAlive")
                        .HasColumnType("boolean");

                    b.Property<string>("MatchId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("MatchId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("PlayerStates", (string)null);
                });

            modelBuilder.Entity("MafiaAPI.Models.Role", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Priority")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Roles", (string)null);
                });

            modelBuilder.Entity("MafiaAPI.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("MafiaAPI.Models.PlayerState", b =>
                {
                    b.HasOne("MafiaAPI.Models.Match", "Match")
                        .WithMany("PlayerStates")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MafiaAPI.Models.Role", "Role")
                        .WithMany("PlayerStates")
                        .HasForeignKey("RoleId");

                    b.HasOne("MafiaAPI.Models.User", "User")
                        .WithMany("PlayerStates")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Match");

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MafiaAPI.Models.Match", b =>
                {
                    b.Navigation("PlayerStates");
                });

            modelBuilder.Entity("MafiaAPI.Models.Role", b =>
                {
                    b.Navigation("PlayerStates");
                });

            modelBuilder.Entity("MafiaAPI.Models.User", b =>
                {
                    b.Navigation("PlayerStates");
                });
#pragma warning restore 612, 618
        }
    }
}