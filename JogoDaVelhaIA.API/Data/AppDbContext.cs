using Microsoft.EntityFrameworkCore;
using JogoDaVelhaIA.Models;

namespace JogoDaVelhaIA.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<JogadaAprendida> JogadasAprendidas { get; set; }
        public DbSet<JogadaBase> JogadasBase { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JogadaAprendida>()
                .HasIndex(j => j.EstadoTabuleiro);

            modelBuilder.Entity<JogadaBase>()
                .HasIndex(j => j.EstadoTabuleiro);
        }
    }
}