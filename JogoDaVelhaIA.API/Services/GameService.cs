using JogoDaVelhIA.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JogoDaVelhIA.Services
{
    public class GameService : IGameService
    {
        private readonly QLearningService _qLearningService;
        private CurrentGameState _currentGameState; // Armazena o estado atual do jogo

        public GameService(QLearningService qLearningService)
        {
            _qLearningService = qLearningService;
            _currentGameState = new CurrentGameState
            {
                Board = new string[9],
                IsPlayerTurn = true,
                Winner = null,
                GameOver = false
            };
        }

        public CurrentGameState GetCurrentState()
        {
            return _currentGameState;
        }

        public Task<CurrentGameState> StartNewGame()
        {
            // Reseta o estado do jogo
            _currentGameState = new CurrentGameState
            {
                Board = new string[9],
                IsPlayerTurn = true,
                Winner = null,
                GameOver = false
            };

            return Task.FromResult(_currentGameState);
        }

        public async Task<CurrentGameState> MakeMove(int position, CurrentGameState currentState)
        {
            // Atualiza o estado interno com o estado recebido
            _currentGameState = currentState;

            // Verifica se o jogo já acabou ou se a posição é inválida
            if (_currentGameState.GameOver || position < 0 || position > 8 ||
                !string.IsNullOrEmpty(_currentGameState.Board[position]))
            {
                return _currentGameState;
            }

            // Faz a jogada do jogador
            _currentGameState.Board[position] = "X";
            _currentGameState.IsPlayerTurn = false;

            // Verifica se o jogador venceu
            if (CheckWin(_currentGameState.Board, "X"))
            {
                _currentGameState.Winner = "X";
                _currentGameState.GameOver = true;
                await _qLearningService.UpdateQLearning(_currentGameState.Board, "X");
                return _currentGameState;
            }

            // Verifica empate
            if (CheckDraw(_currentGameState.Board))
            {
                _currentGameState.GameOver = true;
                await _qLearningService.UpdateQLearning(_currentGameState.Board, "draw");
                return _currentGameState;
            }

            // Se o jogo não acabou, faz a jogada da IA
            return await MakeAIMove(_currentGameState);
        }

        public async Task<CurrentGameState> MakeAIMove(CurrentGameState currentState)
        {
            // Atualiza o estado interno
            _currentGameState = currentState;

            // Escolhe a melhor jogada com Q-Learning
            var aiMove = await _qLearningService.ChooseBestMove(_currentGameState.Board, "O");

            // Faz a jogada da IA
            _currentGameState.Board[aiMove] = "O";
            _currentGameState.IsPlayerTurn = true;

            // Verifica se a IA venceu
            if (CheckWin(_currentGameState.Board, "O"))
            {
                _currentGameState.Winner = "O";
                _currentGameState.GameOver = true;
                await _qLearningService.UpdateQLearning(_currentGameState.Board, "O");
                return _currentGameState;
            }

            // Verifica empate
            if (CheckDraw(_currentGameState.Board))
            {
                _currentGameState.GameOver = true;
                await _qLearningService.UpdateQLearning(_currentGameState.Board, "draw");
            }

            return _currentGameState;
        }

        private bool CheckWin(string[] board, string player)
        {
            // Verifica linhas, colunas e diagonais
            return
                // Linhas
                (board[0] == player && board[1] == player && board[2] == player) ||
                (board[3] == player && board[4] == player && board[5] == player) ||
                (board[6] == player && board[7] == player && board[8] == player) ||
                // Colunas
                (board[0] == player && board[3] == player && board[6] == player) ||
                (board[1] == player && board[4] == player && board[7] == player) ||
                (board[2] == player && board[5] == player && board[8] == player) ||
                // Diagonais
                (board[0] == player && board[4] == player && board[8] == player) ||
                (board[2] == player && board[4] == player && board[6] == player);
        }

        private bool CheckDraw(string[] board)
        {
            return board.All(cell => !string.IsNullOrEmpty(cell));
        }
    }
}