using JogoDaVelhIA.API.Models.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JogoDaVelhIA.API.Data.Configurations
{
    public class HistoricoPartidaConfiguration : IEntityTypeConfiguration<HistoricoPartida>
    {
        public void Configure(EntityTypeBuilder<HistoricoPartida> builder)
        {
            builder.ToTable("HistoricoPartidas");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.NumeroJogada)
                .IsRequired();

            builder.Property(h => h.PosicaoJogada)
                .IsRequired();

            builder.Property(h => h.SimboloJogador)
                .IsRequired()
                .HasMaxLength(1);

            builder.HasOne(h => h.Partida)
                .WithMany(p => p.Historico)
                .HasForeignKey(h => h.PartidaId);

            builder.HasOne(h => h.EstadoTabuleiro)
                .WithMany(e => e.HistoricoPartidas)
                .HasForeignKey(h => h.EstadoTabuleiroId);
        }
    }
}