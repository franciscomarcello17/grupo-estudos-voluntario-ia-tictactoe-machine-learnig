using JogoDaVelhIA.API.Models.Enums;

namespace JogoDaVelhIA.API.Models.Entidades
{
    public abstract class Jogador
    {
        public SimboloJogador Simbolo { get; protected set; }
        public TipoJogador Tipo { get; protected set; }

        public abstract int FazerJogada(string estadoTabuleiro);
    }
}