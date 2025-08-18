using JogoDaVelhIA.API.Models.Entidades;

namespace JogoDaVelhIA.API.Models.Entidades
{
    public class HistoricoPartida
    {
        public int Id { get; set; }
        public int PartidaId { get; set; }
        public int EstadoTabuleiroId { get; set; }
        public int NumeroJogada { get; set; }
        public int PosicaoJogada { get; set; } // 0-8
        public char SimboloJogador { get; set; } // 'X' ou 'O'

        // Navegação
        public virtual Partida Partida { get; set; }
        public virtual EstadoTabuleiro EstadoTabuleiro { get; set; }
    }
}