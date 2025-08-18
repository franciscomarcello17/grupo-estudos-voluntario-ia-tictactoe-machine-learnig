using JogoDaVelhaIA.API.Models.ViewModels;
using JogoDaVelhaIA.Models.ViewModels;
using JogoDaVelhIA.API.Models.Enums;
using JogoDaVelhIA.API.Models.ViewModels;

namespace JogoDaVelhIA.API.Models.Servicos
{
    public interface IJogoService
    {
        JogoViewModel IniciarNovoJogo();
        JogoViewModel FazerJogada(JogadaViewModel jogada);
        JogoViewModel FazerJogadaIA(int partidaId);
        void FinalizarPartida(int partidaId, ResultadoPartida resultado);
        TabuleiroViewModel ObterTabuleiro(int partidaId);
    }
}