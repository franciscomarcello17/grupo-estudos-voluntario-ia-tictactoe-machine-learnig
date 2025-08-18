using JogoDaVelhIA.API.Models.Entidades;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class EstadoTabuleiro
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(17)]
    public string Estado { get; set; }

    [Required]
    public bool Vitoria { get; set; }

    [Required]
    public bool Derrota { get; set; }

    [Required]
    public bool Empate { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,6)")]
    public decimal ValorQ { get; set; }

    // Método para obter o valor como double
    public double ObterValorQDouble() => (double)ValorQ;

    // Método para definir o valor a partir de double
    public void DefinirValorQ(double valor) => ValorQ = (decimal)valor;

    [Required]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;

    public virtual ICollection<HistoricoPartida> HistoricoPartidas { get; set; }
}