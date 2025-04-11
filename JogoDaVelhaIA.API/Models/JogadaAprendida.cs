namespace JogoDaVelhaIA.Models
{
    public class JogadaAprendida
    {
        public int Id { get; set; }
        public string EstadoTabuleiro { get; set; } = string.Empty;
        public int PosicaoEscolhida { get; set; }
        public string Resultado { get; set; } = string.Empty; // "Vitoria", "Derrota", "Empate"
    }
}