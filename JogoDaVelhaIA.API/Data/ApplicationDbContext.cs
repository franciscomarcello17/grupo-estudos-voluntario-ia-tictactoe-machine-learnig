using JogoDaVelhIA.API.Data.Configurations;
using JogoDaVelhIA.API.Models.Entidades;
using Microsoft.EntityFrameworkCore;

namespace JogoDaVelhIA.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<EstadoTabuleiro> EstadosTabuleiro { get; set; }
        public DbSet<HistoricoPartida> HistoricoPartidas { get; set; }
        public DbSet<Partida> Partidas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new EstadoTabuleiroConfiguration());
            modelBuilder.ApplyConfiguration(new HistoricoPartidaConfiguration());
            modelBuilder.ApplyConfiguration(new PartidaConfiguration());
        }
    }
}