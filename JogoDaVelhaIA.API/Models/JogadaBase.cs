namespace JogoDaVelhaIA.Models
{
    public class JogadaBase
    {
        public int Id { get; set; }
        public string EstadoTabuleiro { get; set; } = string.Empty;
        public int PosicaoEscolhida { get; set; }
        public int Peso { get; set; }
    }
}