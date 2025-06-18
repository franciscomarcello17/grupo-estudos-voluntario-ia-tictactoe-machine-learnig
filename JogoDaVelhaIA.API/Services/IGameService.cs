using JogoDaVelhIA.Models;
using System.Threading.Tasks;

namespace JogoDaVelhIA.Services
{
    public interface IGameService
    {
        Task<CurrentGameState> StartNewGame();
        Task<CurrentGameState> MakeMove(int position, CurrentGameState currentState);
        Task<CurrentGameState> MakeAIMove(CurrentGameState currentState);
        CurrentGameState GetCurrentState();
    }
}