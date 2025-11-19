using JogoDaVelhIA.API.Models.Entidades;
using JogoDaVelhIA.API.Models.Enums;
using JogoDaVelhIA.API.Models.Servicos;
using JogoDaVelhIA.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace JogoDaVelhIA.API.Models.Servicos
{
    public class QLearningService : IQLearningService
    {
        private readonly ApplicationDbContext _context;
        private double _taxaAprendizado;
        private double _fatorDesconto;
        private double _epsilon;
        private readonly ILogger<QLearningService> _logger;

        public QLearningService(ApplicationDbContext context, ILogger<QLearningService> logger)
        {
            _context = context;
            _logger = logger;
            InicializarParametros();
        }

        public void InicializarParametros(double taxaAprendizado = 0.1, double fatorDesconto = 0.9, double epsilon = 0.3)
        {
            _taxaAprendizado = taxaAprendizado;
            _fatorDesconto = fatorDesconto;
            _epsilon = epsilon;

            _logger.LogInformation($"Parâmetros inicializados: Alpha={_taxaAprendizado}, Gamma={_fatorDesconto}, Epsilon={_epsilon}");
        }

        public int ObterMelhorJogada(string estadoAtual, char simbolo)
        {
            var random = new Random();
            var posicoesVazias = ObterPosicoesVazias(estadoAtual);

            // Exploração: escolhe uma jogada aleatória com probabilidade epsilon
            if (random.NextDouble() < _epsilon)
            {
                _logger.LogDebug($"Exploração: jogada aleatória para estado {estadoAtual}");
                return posicoesVazias[random.Next(posicoesVazias.Count)];
            }

            // Explotação: escolhe a melhor jogada conhecida
            var estadoNormalizado = NormalizarEstado(estadoAtual);
            var estadoTabuleiro = ObterOuCriarEstado(estadoNormalizado);

            if (estadoTabuleiro.ObterValorQDouble() == 0)
            {
                _logger.LogDebug($"Estado {estadoNormalizado} tem Q=0, usando jogada aleatória");
                return posicoesVazias[random.Next(posicoesVazias.Count)];
            }

            // Simular todas as jogadas possíveis e escolher a melhor
            var melhorJogada = -1;
            var melhorValorQ = double.MinValue;
            var jogadasValidas = new List<(int posicao, double valorQ)>();

            for (int i = 0; i < 9; i++)
            {
                if (estadoAtual[i] == '\0')
                {
                    var novoEstado = estadoAtual.ToCharArray();
                    novoEstado[i] = simbolo;
                    var novoEstadoString = new string(novoEstado);
                    var estadoNormalizadoNovo = NormalizarEstado(novoEstadoString);

                    var estadoNovoTabuleiro = ObterOuCriarEstado(estadoNormalizadoNovo);
                    double valorQ = estadoNovoTabuleiro.ObterValorQDouble();

                    jogadasValidas.Add((i, valorQ));

                    if (valorQ > melhorValorQ)
                    {
                        melhorValorQ = valorQ;
                        melhorJogada = i;
                    }
                }
            }

            _logger.LogDebug($"Melhor jogada para {estadoAtual}: posição {melhorJogada} com Q={melhorValorQ}");
            _logger.LogDebug($"Jogadas válidas: {string.Join(", ", jogadasValidas.Select(j => $"{j.posicao}:{j.valorQ}"))}");

            return melhorJogada != -1 ? melhorJogada : posicoesVazias[random.Next(posicoesVazias.Count)];
        }

        public void AtualizarQValue(string estadoAnterior, int acao, string estadoAtual, double recompensa)
        {
            try
            {
                var estadoAnteriorNormalizado = NormalizarEstado(estadoAnterior);
                var estadoAtualNormalizado = NormalizarEstado(estadoAtual);

                var estadoAnteriorTabuleiro = ObterOuCriarEstado(estadoAnteriorNormalizado);
                var estadoAtualTabuleiro = ObterOuCriarEstado(estadoAtualNormalizado);

                double qAtual = estadoAnteriorTabuleiro.ObterValorQDouble();
                double maxQProximoEstado = estadoAtualTabuleiro.ObterValorQDouble();

                // Fórmula Q-Learning: Q(s,a) ← Q(s,a) + α[r + γ·max(Q(s',a')) - Q(s,a)]
                double novoQValue = qAtual + _taxaAprendizado * (recompensa + _fatorDesconto * maxQProximoEstado - qAtual);

                estadoAnteriorTabuleiro.DefinirValorQ(novoQValue);
                estadoAnteriorTabuleiro.DataAtualizacao = DateTime.UtcNow;

                _logger.LogInformation($"Q-Value atualizado: {estadoAnteriorNormalizado} -> {novoQValue:F4} " +
                                      $"(Q_atual: {qAtual:F4}, Recompensa: {recompensa}, " +
                                      $"MaxQ_next: {maxQProximoEstado:F4})");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao atualizar Q-Value para estado {estadoAnterior}");
            }
        }

        public async Task TreinarIAAsync(int numeroEpisodios)
        {
            var random = new Random();
            var stopwatch = Stopwatch.StartNew();
            var episodiosComSucesso = 0;

            _logger.LogInformation($"Iniciando treinamento com {numeroEpisodios} episódios");

            try
            {
                for (int episodio = 0; episodio < numeroEpisodios; episodio++)
                {
                    try
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

                        _logger.LogDebug($"Episódio {episodio}: Iniciando partida");

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
                                _logger.LogDebug($"Humano (X) joga na posição {posicaoJogada}");
                            }
                            else
                            {
                                // IA (O) faz jogada usando Q-Learning
                                posicaoJogada = ObterMelhorJogada(estadoAtual, 'O');
                                _logger.LogDebug($"IA (O) joga na posição {posicaoJogada}");
                            }

                            // Registrar jogada
                            var novoEstado = estadoAtual.ToCharArray();
                            novoEstado[posicaoJogada] = simboloAtual;
                            estadoAtual = new string(novoEstado);

                            historicoJogadas.Add((estadoAtual, posicaoJogada, simboloAtual));

                            _logger.LogDebug($"Estado atual: {FormatarEstado(estadoAtual)}");

                            // Verificar resultado
                            var resultado = VerificarResultado(estadoAtual);
                            if (resultado != ResultadoPartida.EmAndamento)
                            {
                                partida.Resultado = resultado;
                                partida.JogadorVencedor = resultado == ResultadoPartida.VitoriaHumano ? "Humano" :
                                                     resultado == ResultadoPartida.VitoriaIA ? "IA" : null;
                                partida.DataFim = DateTime.UtcNow;

                                _logger.LogDebug($"Episódio {episodio} finalizado: {resultado}");
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
                            else if (simbolo == 'O') // Jogadas da IA que não são finais
                            {
                                recompensa = 0.1; // Pequena recompensa por jogadas intermediárias
                            }

                            if (simbolo == 'O') // Só atualiza Q-values para jogadas da IA
                            {
                                AtualizarQValue(estado, acao, proximoEstado, recompensa);
                            }
                        }

                        episodiosComSucesso++;

                        // Salvar a cada 100 episódios para melhor performance
                        if (episodio % 100 == 0 && episodio > 0)
                        {
                            await _context.SaveChangesAsync();
                            _context.ChangeTracker.Clear();

                            _logger.LogInformation($"Episódio {episodio} concluído. " +
                                                  $"Resultado: {partida.Resultado}. " +
                                                  $"Estados no banco: {_context.EstadosTabuleiro.Count()}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Erro no episódio {episodio}");
                    }
                }

                // Salvar quaisquer alterações restantes
                await _context.SaveChangesAsync();

                stopwatch.Stop();

                _logger.LogInformation($"Treinamento concluído! " +
                                      $"Episódios: {episodiosComSucesso}/{numeroEpisodios} " +
                                      $"Tempo: {stopwatch.Elapsed.TotalMinutes:F2} minutos " +
                                      $"Estados aprendidos: {_context.EstadosTabuleiro.Count()}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante o treinamento da IA");
                throw;
            }
        }

        public string NormalizarEstado(string estado)
        {
            try
            {
                // Primeiro, normalizar o estado de entrada
                var estadoNormalizado = estado.Replace("\0", " ").Replace(",", "");

                // Se o estado já está no formato com vírgulas, converter para formato sem vírgulas
                //if (estado.Contains(','))
                //{
                //    estadoNormalizado = string.Join("", estado.Split(',').Select(s => s == "0" ? " " : s));
                //}

                var tabuleiro = estadoNormalizado.ToCharArray();

                // Garantir que temos exatamente 9 caracteres
                if (tabuleiro.Length != 9)
                {
                    tabuleiro = new char[9];
                    Array.Fill(tabuleiro, ' ');
                }

                var simetrias = GerarSimetrias(tabuleiro);

                // Converter cada simetria para string formatada corretamente
                var simetriasFormatadas = simetrias
                    .Select(s => string.Join(",", s.Select(c => c == ' ' ? "0" : c.ToString())))
                    .ToList();

                var estadoCanonico = simetriasFormatadas
                    .OrderBy(s => s)
                    .First();

                _logger.LogTrace($"Estado original: {estado} -> Normalizado: {estadoCanonico}");

                return estado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao normalizar estado: {estado}");
                // Fallback: retornar estado padrão
                return "0,0,0,0,0,0,0,0,0";
            }
        }
        private EstadoTabuleiro ObterOuCriarEstado(string estadoNormalizado)
        {
            var estado = _context.EstadosTabuleiro
                .FirstOrDefault(e => e.Estado == estadoNormalizado);

            if (estado == null)
            {
                estado = new EstadoTabuleiro
                {
                    Estado = estadoNormalizado,
                    Vitoria = false,
                    Derrota = false,
                    Empate = false,
                    DataCriacao = DateTime.UtcNow,
                    DataAtualizacao = DateTime.UtcNow
                };
                estado.DefinirValorQ(0);
                _context.EstadosTabuleiro.Add(estado);

                _logger.LogDebug($"Novo estado criado: {estadoNormalizado}");
            }

            return estado;
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

        private string FormatarEstado(string estado)
        {
            var chars = estado.ToCharArray();
            return $"[{chars[0]}|{chars[1]}|{chars[2]}]\n[{chars[3]}|{chars[4]}|{chars[5]}]\n[{chars[6]}|{chars[7]}|{chars[8]}]";
        }
    }
}