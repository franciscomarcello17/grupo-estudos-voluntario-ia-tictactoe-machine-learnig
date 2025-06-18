using JogoDaVelhIA.Models;
using Microsoft.EntityFrameworkCore;

namespace JogoDaVelhIA.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<GameState> GameStates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameState>()
                .HasKey(gs => gs.StateHash);

            modelBuilder.Entity<GameState>()
                .Property(gs => gs.StateHash)
                .HasMaxLength(64);
        }
    }
}