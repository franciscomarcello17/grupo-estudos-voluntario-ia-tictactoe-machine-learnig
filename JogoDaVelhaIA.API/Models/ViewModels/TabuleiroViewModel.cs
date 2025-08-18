using JogoDaVelhIA.API.Models.Enums;

namespace JogoDaVelhIA.API.Models.ViewModels
{
    public class TabuleiroViewModel
    {
        public int PartidaId { get; set; }
        public string[] Posicoes { get; set; }
        public ResultadoPartida Resultado { get; set; }
        public SimboloJogador VezDoJogador { get; set; }
    }
}