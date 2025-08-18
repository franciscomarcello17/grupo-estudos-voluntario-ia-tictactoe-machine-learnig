using JogoDaVelhIA.API.Models.Entidades;
using JogoDaVelhIA.API.Models.Enums;
using JogoDaVelhIA.API.Models.Servicos;
using JogoDaVelhIA.Data;
using Microsoft.EntityFrameworkCore;

namespace JogoDaVelhIA.API.Models.Servicos
{
    public class QLearningService : IQLearningService
    {
        private readonly ApplicationDbContext _context;
        private double _taxaAprendizado;
        private double _fatorDesconto;
        private double _epsilon;

        public QLearningService(ApplicationDbContext context)
        {
            _context = context;
            InicializarParametros();
        }

        public void InicializarParametros(double taxaAprendizado = 0.1, double fatorDesconto = 0.9, double epsilon = 0.3)
        {
            _taxaAprendizado = taxaAprendizado;
            _fatorDesconto = fatorDesconto;
            _epsilon = epsilon;
        }

        public int ObterMelhorJogada(string estadoAtual, char simbolo)
        {
            var random = new Random();

            // Exploração: escolhe uma jogada aleatória com probabilidade epsilon
            if (random.NextDouble() < _epsilon)
            {
                var posicoesVazias = ObterPosicoesVazias(estadoAtual);
                return posicoesVazias[random.Next(posicoesVazias.Count)];
            }

            // Explotação: escolhe a melhor jogada conhecida
            var estadoNormalizado = NormalizarEstado(estadoAtual);
            var estadoTabuleiro = _context.EstadosTabuleiro
                .FirstOrDefault(e => e.Estado == estadoNormalizado);

            if (estadoTabuleiro == null || estadoTabuleiro.ValorQ == 0)
            {
                var posicoesVazias = ObterPosicoesVazias(estadoAtual);
                return posicoesVazias[random.Next(posicoesVazias.Count)];
            }

            // Simular todas as jogadas possíveis e escolher a melhor
            var melhorJogada = -1;
            var melhorValorQ = double.MinValue;

            for (int i = 0; i < 9; i++)
            {
                if (estadoAtual[i] == '\0')
                {
                    var novoEstado = estadoAtual.ToCharArray();
                    novoEstado[i] = simbolo;
                    var novoEstadoString = new string(novoEstado);
                    var estadoNormalizadoNovo = NormalizarEstado(novoEstadoString);

                    var estadoNovoTabuleiro = _context.EstadosTabuleiro
                        .FirstOrDefault(e => e.Estado == estadoNormalizadoNovo);

                    double valorQ = estadoNovoTabuleiro?.ObterValorQDouble() ?? 0; // Usando o novo método

                    if (valorQ > melhorValorQ)
                    {
                        melhorValorQ = valorQ;
                        melhorJogada = i;
                    }
                }
            }

            return melhorJogada;
        }


        public void AtualizarQValue(string estadoAnterior, int acao, string estadoAtual, double recompensa)
        {
            var estadoAnteriorNormalizado = NormalizarEstado(estadoAnterior);
            var estadoAtualNormalizado = NormalizarEstado(estadoAtual);

            var estadoAnteriorTabuleiro = _context.EstadosTabuleiro
                .FirstOrDefault(e => e.Estado == estadoAnteriorNormalizado);

            var estadoAtualTabuleiro = _context.EstadosTabuleiro
                .FirstOrDefault(e => e.Estado == estadoAtualNormalizado);

            if (estadoAnteriorTabuleiro == null)
            {
                estadoAnteriorTabuleiro = new EstadoTabuleiro
                {
                    Estado = estadoAnteriorNormalizado,
                    Vitoria = false,
                    Derrota = false,
                    Empate = false,
                    DataCriacao = DateTime.UtcNow,
                    DataAtualizacao = DateTime.UtcNow
                };
                estadoAnteriorTabuleiro.DefinirValorQ(0); // Usando o novo método
                _context.EstadosTabuleiro.Add(estadoAnteriorTabuleiro);
            }

            double maxQProximoEstado = estadoAtualTabuleiro?.ObterValorQDouble() ?? 0; // Usando o novo método
            double qAtual = estadoAnteriorTabuleiro.ObterValorQDouble(); // Usando o novo método

            // Fórmula Q-Learning: Q(s,a) ← Q(s,a) + α[r + γ·max(Q(s',a')) - Q(s,a)]
            double novoQValue = qAtual + _taxaAprendizado * (recompensa + _fatorDesconto * maxQProximoEstado - qAtual);

            estadoAnteriorTabuleiro.DefinirValorQ(novoQValue); // Usando o novo método
            estadoAnteriorTabuleiro.DataAtualizacao = DateTime.UtcNow;

            _context.SaveChanges();
        }

        public async Task TreinarIAAsync(int numeroEpisodios)
        {
            var random = new Random();

            for (int episodio = 0; episodio < numeroEpisodios; episodio++)
            {
                // Iniciar nova partida
                var partida = new Partida
                {
                    DataInicio = DateTime.UtcNow,
                    Resultado = ResultadoPartida.EmAndamento
                };
                _context.Partidas.Add(partida);

                string estadoAtual = new string('\0', 9);
                var historicoJogadas = new List<(string estado, int acao, char simbolo)>();

                while (true)
                {
                    // Determinar quem joga (X ou O)
                    char simboloAtual = estadoAtual.Count(c => c == 'X' || c == 'O') % 2 == 0 ? 'X' : 'O';

                    int posicaoJogada;
                    if (simboloAtual == 'X')
                    {
                        // Jogador X (humano simulado) faz jogada aleatória
                        var posicoesVazias = ObterPosicoesVazias(estadoAtual);
                        posicaoJogada = posicoesVazias[random.Next(posicoesVazias.Count)];
                    }
                    else
                    {
                        // IA (O) faz jogada usando Q-Learning
                        posicaoJogada = ObterMelhorJogada(estadoAtual, 'O');
                    }

                    // Registrar jogada
                    var novoEstado = estadoAtual.ToCharArray();
                    novoEstado[posicaoJogada] = simboloAtual;
                    estadoAtual = new string(novoEstado);

                    historicoJogadas.Add((estadoAtual, posicaoJogada, simboloAtual));

                    // Verificar resultado
                    var resultado = VerificarResultado(estadoAtual);
                    if (resultado != ResultadoPartida.EmAndamento)
                    {
                        partida.Resultado = resultado;
                        partida.JogadorVencedor = resultado == ResultadoPartida.VitoriaHumano ? "Humano" :
                                                 resultado == ResultadoPartida.VitoriaIA ? "IA" : null;
                        partida.DataFim = DateTime.UtcNow;
                        break;
                    }
                }

                // Atualizar Q-values com base no resultado
                for (int i = 0; i < historicoJogadas.Count; i++)
                {
                    var (estado, acao, simbolo) = historicoJogadas[i];
                    string proximoEstado = i < historicoJogadas.Count - 1 ? historicoJogadas[i + 1].estado : estado;

                    double recompensa = 0;
                    if (i == historicoJogadas.Count - 1) // Última jogada
                    {
                        recompensa = partida.Resultado == ResultadoPartida.VitoriaIA ? 1 :
                                    partida.Resultado == ResultadoPartida.VitoriaHumano ? -1 : 0;
                    }

                    AtualizarQValue(estado, acao, proximoEstado, recompensa);
                }

                await _context.SaveChangesAsync();

                if (episodio % 100 == 0)
                {
                    Console.WriteLine($"Episódio {episodio} concluído. Resultado: {partida.Resultado}");
                }
            }
        }

        public string NormalizarEstado(string estado)
        {
            // Converter para formato padrão: "X,O,,X,,O,,,"
            var tabuleiro = estado.ToCharArray();
            var tabuleiroString = string.Join(",", tabuleiro.Select(c => c == '\0' ? "" : c.ToString()));

            // Gerar todas as simetrias possíveis
            var simetrias = GerarSimetrias(tabuleiro);

            // Selecionar a representação canônica (menor valor hash)
            var estadoCanonico = simetrias
                .OrderBy(s => s.GetHashCode())
                .First();

            return string.Join(",", estadoCanonico.Select(c => c == '\0' ? "" : c.ToString()));
        }

        private List<int> ObterPosicoesVazias(string estado)
        {
            var posicoesVazias = new List<int>();
            for (int i = 0; i < estado.Length; i++)
            {
                if (estado[i] == '\0')
                {
                    posicoesVazias.Add(i);
                }
            }
            return posicoesVazias;
        }

        private ResultadoPartida VerificarResultado(string estado)
        {
            // Verificar linhas
            for (int i = 0; i < 9; i += 3)
            {
                if (estado[i] != '\0' && estado[i] == estado[i + 1] && estado[i] == estado[i + 2])
                {
                    return estado[i] == 'X' ? ResultadoPartida.VitoriaHumano : ResultadoPartida.VitoriaIA;
                }
            }

            // Verificar colunas
            for (int i = 0; i < 3; i++)
            {
                if (estado[i] != '\0' && estado[i] == estado[i + 3] && estado[i] == estado[i + 6])
                {
                    return estado[i] == 'X' ? ResultadoPartida.VitoriaHumano : ResultadoPartida.VitoriaIA;
                }
            }

            // Verificar diagonais
            if (estado[0] != '\0' && estado[0] == estado[4] && estado[0] == estado[8])
            {
                return estado[0] == 'X' ? ResultadoPartida.VitoriaHumano : ResultadoPartida.VitoriaIA;
            }

            if (estado[2] != '\0' && estado[2] == estado[4] && estado[2] == estado[6])
            {
                return estado[2] == 'X' ? ResultadoPartida.VitoriaHumano : ResultadoPartida.VitoriaIA;
            }

            // Verificar empate
            if (!estado.Contains('\0'))
            {
                return ResultadoPartida.Empate;
            }

            return ResultadoPartida.EmAndamento;
        }

        private List<char[]> GerarSimetrias(char[] tabuleiro)
        {
            var simetrias = new List<char[]>();

            // Original
            simetrias.Add((char[])tabuleiro.Clone());

            // Rotação 90°
            var rot90 = new char[9];
            rot90[0] = tabuleiro[6];
            rot90[1] = tabuleiro[3];
            rot90[2] = tabuleiro[0];
            rot90[3] = tabuleiro[7];
            rot90[4] = tabuleiro[4];
            rot90[5] = tabuleiro[1];
            rot90[6] = tabuleiro[8];
            rot90[7] = tabuleiro[5];
            rot90[8] = tabuleiro[2];
            simetrias.Add(rot90);

            // Rotação 180°
            var rot180 = new char[9];
            rot180[0] = tabuleiro[8];
            rot180[1] = tabuleiro[7];
            rot180[2] = tabuleiro[6];
            rot180[3] = tabuleiro[5];
            rot180[4] = tabuleiro[4];
            rot180[5] = tabuleiro[3];
            rot180[6] = tabuleiro[2];
            rot180[7] = tabuleiro[1];
            rot180[8] = tabuleiro[0];
            simetrias.Add(rot180);

            // Rotação 270°
            var rot270 = new char[9];
            rot270[0] = tabuleiro[2];
            rot270[1] = tabuleiro[5];
            rot270[2] = tabuleiro[8];
            rot270[3] = tabuleiro[1];
            rot270[4] = tabuleiro[4];
            rot270[5] = tabuleiro[7];
            rot270[6] = tabuleiro[0];
            rot270[7] = tabuleiro[3];
            rot270[8] = tabuleiro[6];
            simetrias.Add(rot270);

            // Espelhamento horizontal
            var espH = new char[9];
            espH[0] = tabuleiro[2];
            espH[1] = tabuleiro[1];
            espH[2] = tabuleiro[0];
            espH[3] = tabuleiro[5];
            espH[4] = tabuleiro[4];
            espH[5] = tabuleiro[3];
            espH[6] = tabuleiro[8];
            espH[7] = tabuleiro[7];
            espH[8] = tabuleiro[6];
            simetrias.Add(espH);

            // Espelhamento vertical
            var espV = new char[9];
            espV[0] = tabuleiro[6];
            espV[1] = tabuleiro[7];
            espV[2] = tabuleiro[8];
            espV[3] = tabuleiro[3];
            espV[4] = tabuleiro[4];
            espV[5] = tabuleiro[5];
            espV[6] = tabuleiro[0];
            espV[7] = tabuleiro[1];
            espV[8] = tabuleiro[2];
            simetrias.Add(espV);

            // Espelhamento diagonal principal
            var espD1 = new char[9];
            espD1[0] = tabuleiro[0];
            espD1[1] = tabuleiro[3];
            espD1[2] = tabuleiro[6];
            espD1[3] = tabuleiro[1];
            espD1[4] = tabuleiro[4];
            espD1[5] = tabuleiro[7];
            espD1[6] = tabuleiro[2];
            espD1[7] = tabuleiro[5];
            espD1[8] = tabuleiro[8];
            simetrias.Add(espD1);

            // Espelhamento diagonal secundária
            var espD2 = new char[9];
            espD2[0] = tabuleiro[8];
            espD2[1] = tabuleiro[5];
            espD2[2] = tabuleiro[2];
            espD2[3] = tabuleiro[7];
            espD2[4] = tabuleiro[4];
            espD2[5] = tabuleiro[1];
            espD2[6] = tabuleiro[6];
            espD2[7] = tabuleiro[3];
            espD2[8] = tabuleiro[0];
            simetrias.Add(espD2);

            return simetrias;
        }
    }
}