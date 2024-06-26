﻿// <auto-generated />
using System;
using MafiaAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MafiaAPI.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240605195211_mig050624_2251")]
    partial class mig050624_2251
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.HasKey("Id");

                    b.ToTable("Matches");
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

                    b.ToTable("PlayerStates");
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

                    b.HasKey("Id");

                    b.ToTable("Roles");
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

                    b.ToTable("Users");
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
