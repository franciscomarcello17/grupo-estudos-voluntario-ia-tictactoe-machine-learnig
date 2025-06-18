using System;
using System.Linq;

namespace JogoDaVelhIA.Models
{
    public static class Transformations
    {
        // Gera todas as transformações possíveis de um tabuleiro
        public static string[] GetAllTransformations(string[] board)
        {
            return new string[]
            {
                GetBoardString(board), // Original
                GetBoardString(Rotate90(board)),
                GetBoardString(Rotate180(board)),
                GetBoardString(Rotate270(board)),
                GetBoardString(MirrorHorizontal(board)),
                GetBoardString(MirrorVertical(board)),
                GetBoardString(MirrorDiagonal(board)),
                GetBoardString(MirrorAntiDiagonal(board))
            };
        }

        // Encontra o estado canônico (menor hash)
        public static string FindCanonicalState(string[] board)
        {
            var transformations = GetAllTransformations(board);
            return transformations.OrderBy(t => t.GetHashCode()).First();
        }

        // Converte array para string
        public static string GetBoardString(string[] board)
        {
            return string.Join(",", board.Select(c => string.IsNullOrEmpty(c) ? "" : c));
        }

        // Converte string para array
        public static string[] GetBoardArray(string boardString)
        {
            return boardString.Split(',').Select(s => string.IsNullOrEmpty(s) ? null : s).ToArray();
        }

        // Métodos de transformação
        public static string[] Rotate90(string[] board)
        {
            return new string[]
            {
                board[6], board[3], board[0],
                board[7], board[4], board[1],
                board[8], board[5], board[2]
            };
        }

        public static string[] Rotate180(string[] board)
        {
            return new string[]
            {
                board[8], board[7], board[6],
                board[5], board[4], board[3],
                board[2], board[1], board[0]
            };
        }

        public static string[] Rotate270(string[] board)
        {
            return new string[]
            {
                board[2], board[5], board[8],
                board[1], board[4], board[7],
                board[0], board[3], board[6]
            };
        }

        public static string[] MirrorHorizontal(string[] board)
        {
            return new string[]
            {
                board[6], board[7], board[8],
                board[3], board[4], board[5],
                board[0], board[1], board[2]
            };
        }

        public static string[] MirrorVertical(string[] board)
        {
            return new string[]
            {
                board[2], board[1], board[0],
                board[5], board[4], board[3],
                board[8], board[7], board[6]
            };
        }

        public static string[] MirrorDiagonal(string[] board)
        {
            return new string[]
            {
                board[0], board[3], board[6],
                board[1], board[4], board[7],
                board[2], board[5], board[8]
            };
        }

        public static string[] MirrorAntiDiagonal(string[] board)
        {
            return new string[]
            {
                board[8], board[5], board[2],
                board[7], board[4], board[1],
                board[6], board[3], board[0]
            };
        }

        // Normaliza o tabuleiro para a perspectiva do O
        public static string[] NormalizeToO(string[] board, string currentPlayer)
        {
            if (currentPlayer == "O") return board;

            return board.Select(c =>
                c == "X" ? "O" :
                c == "O" ? "X" :
                null).ToArray();
        }
    }
}