using JogoDaVelhIA.Data;
using JogoDaVelhIA.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JogoDaVelhIA.Services
{
    public class QLearningService
    {
        private readonly ApplicationDbContext _context;
        private readonly QLearningParameters _parameters;

        public QLearningService(ApplicationDbContext context, QLearningParameters parameters)
        {
            _context = context;
            _parameters = parameters;
        }

        // Escolhe a melhor jogada com base no Q-Learning
        public async Task<int> ChooseBestMove(string[] board, string player)
        {
            // Normaliza o tabuleiro para a perspectiva do O
            var normalizedBoard = Transformations.NormalizeToO(board, player);
            var canonicalState = Transformations.FindCanonicalState(normalizedBoard);

            // Busca o estado no banco de dados
            var state = await _context.GameStates
                .FirstOrDefaultAsync(s => s.StateHash == canonicalState.GetHashCode().ToString());

            // Se não existir no banco, cria um novo estado
            if (state == null)
            {
                state = new GameState
                {
                    StateHash = canonicalState.GetHashCode().ToString(),
                    BoardState = canonicalState
                };
                _context.GameStates.Add(state);
                await _context.SaveChangesAsync();
            }

            // Encontra todas as posições vazias
            var emptyPositions = normalizedBoard
                .Select((value, index) => new { value, index })
                .Where(x => string.IsNullOrEmpty(x.value))
                .Select(x => x.index)
                .ToList();

            // Se não houver movimentos possíveis
            if (!emptyPositions.Any()) return -1;

            // Com probabilidade ExplorationRate, escolhe um movimento aleatório
            if (new Random().NextDouble() < _parameters.ExplorationRate)
            {
                return emptyPositions[new Random().Next(emptyPositions.Count)];
            }

            // Caso contrário, escolhe o melhor movimento com base no Q-Learning
            // Aqui implementaríamos a lógica para escolher o movimento com maior recompensa esperada
            // Por simplicidade, vamos escolher aleatoriamente por enquanto
            return emptyPositions[new Random().Next(emptyPositions.Count)];
        }

        // Atualiza o Q-Learning com o resultado do jogo
        public async Task UpdateQLearning(string[] board, string winner)
        {
            // Normaliza para a perspectiva do O
            var normalizedBoard = Transformations.NormalizeToO(board, "X");
            var canonicalState = Transformations.FindCanonicalState(normalizedBoard);

            var state = await _context.GameStates
                .FirstOrDefaultAsync(s => s.StateHash == canonicalState.GetHashCode().ToString());

            if (state == null)
            {
                state = new GameState
                {
                    StateHash = canonicalState.GetHashCode().ToString(),
                    BoardState = canonicalState
                };
                _context.GameStates.Add(state);
            }

            // Atualiza as estatísticas com base no vencedor
            if (winner == "O")
            {
                state.Wins++;
            }
            else if (winner == "X")
            {
                state.Losses++;
            }
            else
            {
                state.Draws++;
            }

            state.LastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}