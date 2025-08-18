using JogoDaVelhIA.API.Models.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JogoDaVelhIA.API.Data.Configurations
{
    public class EstadoTabuleiroConfiguration : IEntityTypeConfiguration<EstadoTabuleiro>
    {
        public void Configure(EntityTypeBuilder<EstadoTabuleiro> builder)
        {
            builder.ToTable("EstadosTabuleiro");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Estado)
                .IsRequired()
                .HasMaxLength(17);

            builder.Property(e => e.ValorQ)
                .HasColumnType("decimal(18,6)");

            builder.Property(e => e.DataCriacao)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.DataAtualizacao)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(e => e.Estado)
                .IsUnique();
        }
    }
}