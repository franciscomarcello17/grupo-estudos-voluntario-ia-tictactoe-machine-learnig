namespace JogoDaVelhaIA.DTOs
{
    public class JogadaRequest
    {
        public string EstadoTabuleiro { get; set; } = string.Empty;
        public int PosicaoEscolhida { get; set; }
        public string Resultado { get; set; } = string.Empty;
    }
}