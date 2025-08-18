using JogoDaVelhIA.API.Models.Enums;

namespace JogoDaVelhaIA.Models.ViewModels
{
    public class JogoViewModel
    {
        public int PartidaId { get; set; }
        public string[] Tabuleiro { get; set; }
        public SimboloJogador VezDoJogador { get; set; }
        public ResultadoPartida Resultado { get; set; }
        public string Mensagem { get; set; }
    }
}