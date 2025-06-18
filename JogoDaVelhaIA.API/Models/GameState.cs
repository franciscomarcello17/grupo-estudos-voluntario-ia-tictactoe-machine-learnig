using System.ComponentModel.DataAnnotations;

namespace JogoDaVelhIA.Models
{
    public class GameState
    {
        [Key]
        public string StateHash { get; set; }  // Hash do estado canônico

        [Required]
        public string BoardState { get; set; }  // Estado do tabuleiro no formato "X,O,,X,,O,,"

        public int Wins { get; set; } = 0;     // Vitórias para O (estado canônico)
        public int Losses { get; set; } = 0;   // Derrotas para O (X venceu)
        public int Draws { get; set; } = 0;    // Empates

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    public class CurrentGameState
    {
        public string[] Board { get; set; } = new string[9];
        public bool IsPlayerTurn { get; set; }
        public string? Winner { get; set; }
        public bool GameOver { get; set; }
    }
}