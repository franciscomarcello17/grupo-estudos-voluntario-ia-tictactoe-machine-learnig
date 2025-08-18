using System.Linq;

namespace JogoDaVelhIA.API.Extensions
{
    public static class TabuleiroExtensions
    {
        public static string ConverterParaString(this char[] tabuleiro)
        {
            return string.Join(",", tabuleiro.Select(c => c == '\0' ? "" : c.ToString()));
        }

        public static char[] ConverterParaArray(this string estado)
        {
            return estado.Split(',')
                .Select(s => string.IsNullOrEmpty(s) ? '\0' : s[0])
                .ToArray();
        }

        public static bool EhPosicaoValida(this int posicao)
        {
            return posicao >= 0 && posicao < 9;
        }

        public static bool EhEstadoFinal(this string estado)
        {
            // Verificar linhas
            for (int i = 0; i < 9; i += 3)
            {
                if (estado[i] != '\0' && estado[i] == estado[i + 1] && estado[i] == estado[i + 2])
                    return true;
            }

            // Verificar colunas
            for (int i = 0; i < 3; i++)
            {
                if (estado[i] != '\0' && estado[i] == estado[i + 3] && estado[i] == estado[i + 6])
                    return true;
            }

            // Verificar diagonais
            if (estado[0] != '\0' && estado[0] == estado[4] && estado[0] == estado[8])
                return true;

            if (estado[2] != '\0' && estado[2] == estado[4] && estado[2] == estado[6])
                return true;

            // Verificar empate
            return !estado.Contains('\0');
        }

        public static string ObterEstadoCanonico(this string estado)
        {
            var tabuleiro = estado.ConverterParaArray();
            var rotacoes = new[]
            {
                tabuleiro, // Original
                Rotacionar90(tabuleiro),
                Rotacionar180(tabuleiro),
                Rotacionar270(tabuleiro),
                EspelharHorizontal(tabuleiro),
                EspelharVertical(tabuleiro),
                EspelharDiagonalPrincipal(tabuleiro),
                EspelharDiagonalSecundaria(tabuleiro)
            };

            return rotacoes
                .Select(t => t.ConverterParaString())
                .OrderBy(s => s)
                .First();
        }

        private static char[] Rotacionar90(char[] tabuleiro)
        {
            var resultado = new char[9];
            resultado[0] = tabuleiro[6];
            resultado[1] = tabuleiro[3];
            resultado[2] = tabuleiro[0];
            resultado[3] = tabuleiro[7];
            resultado[4] = tabuleiro[4];
            resultado[5] = tabuleiro[1];
            resultado[6] = tabuleiro[8];
            resultado[7] = tabuleiro[5];
            resultado[8] = tabuleiro[2];
            return resultado;
        }

        private static char[] Rotacionar180(char[] tabuleiro)
        {
            var resultado = new char[9];
            resultado[0] = tabuleiro[8];
            resultado[1] = tabuleiro[7];
            resultado[2] = tabuleiro[6];
            resultado[3] = tabuleiro[5];
            resultado[4] = tabuleiro[4];
            resultado[5] = tabuleiro[3];
            resultado[6] = tabuleiro[2];
            resultado[7] = tabuleiro[1];
            resultado[8] = tabuleiro[0];
            return resultado;
        }

        private static char[] Rotacionar270(char[] tabuleiro)
        {
            var resultado = new char[9];
            resultado[0] = tabuleiro[2];
            resultado[1] = tabuleiro[5];
            resultado[2] = tabuleiro[8];
            resultado[3] = tabuleiro[1];
            resultado[4] = tabuleiro[4];
            resultado[5] = tabuleiro[7];
            resultado[6] = tabuleiro[0];
            resultado[7] = tabuleiro[3];
            resultado[8] = tabuleiro[6];
            return resultado;
        }

        private static char[] EspelharHorizontal(char[] tabuleiro)
        {
            var resultado = new char[9];
            resultado[0] = tabuleiro[2];
            resultado[1] = tabuleiro[1];
            resultado[2] = tabuleiro[0];
            resultado[3] = tabuleiro[5];
            resultado[4] = tabuleiro[4];
            resultado[5] = tabuleiro[3];
            resultado[6] = tabuleiro[8];
            resultado[7] = tabuleiro[7];
            resultado[8] = tabuleiro[6];
            return resultado;
        }

        private static char[] EspelharVertical(char[] tabuleiro)
        {
            var resultado = new char[9];
            resultado[0] = tabuleiro[6];
            resultado[1] = tabuleiro[7];
            resultado[2] = tabuleiro[8];
            resultado[3] = tabuleiro[3];
            resultado[4] = tabuleiro[4];
            resultado[5] = tabuleiro[5];
            resultado[6] = tabuleiro[0];
            resultado[7] = tabuleiro[1];
            resultado[8] = tabuleiro[2];
            return resultado;
        }

        private static char[] EspelharDiagonalPrincipal(char[] tabuleiro)
        {
            var resultado = new char[9];
            resultado[0] = tabuleiro[0];
            resultado[1] = tabuleiro[3];
            resultado[2] = tabuleiro[6];
            resultado[3] = tabuleiro[1];
            resultado[4] = tabuleiro[4];
            resultado[5] = tabuleiro[7];
            resultado[6] = tabuleiro[2];
            resultado[7] = tabuleiro[5];
            resultado[8] = tabuleiro[8];
            return resultado;
        }

        private static char[] EspelharDiagonalSecundaria(char[] tabuleiro)
        {
            var resultado = new char[9];
            resultado[0] = tabuleiro[8];
            resultado[1] = tabuleiro[5];
            resultado[2] = tabuleiro[2];
            resultado[3] = tabuleiro[7];
            resultado[4] = tabuleiro[4];
            resultado[5] = tabuleiro[1];
            resultado[6] = tabuleiro[6];
            resultado[7] = tabuleiro[3];
            resultado[8] = tabuleiro[0];
            return resultado;
        }
    }
}