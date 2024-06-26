﻿using MafiaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MafiaAPI.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Match> Matches { get; set; } = null!;
        public DbSet<PlayerState> PlayerStates { get; set; }
        public DbSet<Role> Roles { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            // Затычка (без миграции)
            //Database.EnsureDeleted();
            //Database.EnsureCreated();

            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(x => x.Name);

            modelBuilder.Entity<PlayerState>()
                .HasIndex(x => x.MatchId);

            modelBuilder.Entity<PlayerState>()
                .HasOne(x => x.User)
                .WithMany(p => p.PlayerStates)
                .HasForeignKey(x => x.UserId);
            
            modelBuilder.Entity<PlayerState>()
                .HasOne(x => x.Match)
                .WithMany(p => p.PlayerStates)
                .HasForeignKey(x => x.MatchId);

            modelBuilder.Entity<PlayerState>()
                .HasOne(x => x.Role)
                .WithMany(p => p.PlayerStates)
                .HasForeignKey(x => x.RoleId);

            
        }
    }
}
