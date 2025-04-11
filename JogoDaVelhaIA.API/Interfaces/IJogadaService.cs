using JogoDaVelhaIA.DTOs;
using JogoDaVelhaIA.Models;

namespace JogoDaVelhaIA.Interfaces
{
    public interface IJogadaService
    {
        IEnumerable<JogadaAprendida> ObterTodasJogadasAprendidas();
        void AdicionarJogadaAprendida(JogadaRequest jogada);
        void LimparJogadasAprendidas();
        IEnumerable<JogadaBase> ObterTodasJogadasBase();
        JogadaResponse ObterMelhorJogadaBase(JogadaRequest jogada);
    }
}