using JogoDaVelhaIA.Models;

namespace JogoDaVelhaIA.Interfaces
{
    public interface IJogadaRepository
    {
        IEnumerable<JogadaAprendida> GetAllAprendidas();
        void AddAprendida(JogadaAprendida jogada);
        void ClearAprendidas();
        IEnumerable<JogadaBase> GetAllBase();
        int GetBestMoveFromBase(string estado);
    }
}