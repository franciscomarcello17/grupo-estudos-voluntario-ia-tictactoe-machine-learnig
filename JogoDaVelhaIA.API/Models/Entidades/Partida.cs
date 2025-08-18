using JogoDaVelhIA.API.Models.Entidades;
using JogoDaVelhIA.API.Models.Enums;

namespace JogoDaVelhIA.API.Models.Entidades
{
    public class Partida
    {
        public int Id { get; set; }
        public DateTime DataInicio { get; set; } = DateTime.UtcNow;
        public DateTime? DataFim { get; set; }
        public ResultadoPartida Resultado { get; set; }
        public string? JogadorVencedor { get; set; } // "Humano" ou "IA"

        // Navegação
        public virtual ICollection<HistoricoPartida> Historico { get; set; }
    }
}