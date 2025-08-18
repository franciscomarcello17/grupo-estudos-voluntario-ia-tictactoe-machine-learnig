using JogoDaVelhIA.API.Models.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JogoDaVelhIA.API.Data.Configurations
{
    public class PartidaConfiguration : IEntityTypeConfiguration<Partida>
    {
        public void Configure(EntityTypeBuilder<Partida> builder)
        {
            builder.ToTable("Partidas");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.DataInicio)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.Resultado)
                .IsRequired();

            builder.Property(p => p.JogadorVencedor)
                .HasMaxLength(50);
        }
    }
}