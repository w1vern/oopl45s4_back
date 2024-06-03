using MafiaAPI.Models;
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
            // Затычка (без миграции)
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<User>()
                .HasIndex(x => x.Name);

            modelBuilder.Entity<PlayerState>()
                .HasOne(x => x.User)
                .WithMany(p => p.PlayerStates)
                .HasForeignKey(x => x.UserId);
            
            modelBuilder.Entity<PlayerState>()
                .HasOne(x => x.Match)
                .WithMany(p => p.PlayerStates)
                .HasForeignKey(x => x.MatchId);

            /*modelBuilder.Entity<Match>()
                .HasMany(x => x.Users)
                .WithMany(x => x.Matches)
                .UsingEntity(j => j.ToTable("UserMatches"));*/

            /*modelBuilder.Entity<User>()
                .HasOne(u => u.Company)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CompanyInfoKey);*/
        }
    }
}
