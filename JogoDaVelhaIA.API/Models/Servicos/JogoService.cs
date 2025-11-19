using JogoDaVelhaIA.API.Models.ViewModels;
using JogoDaVelhaIA.Models.ViewModels;
using JogoDaVelhIA.API.Models.Entidades;
using JogoDaVelhIA.API.Models.Enums;
using JogoDaVelhIA.API.Models.Servicos;
using JogoDaVelhIA.API.Models.ViewModels;
using JogoDaVelhIA.Data;
using JogoDaVelhIA.Models.Excecoes;
using Microsoft.EntityFrameworkCore;

namespace JogoDaVelhIA.API.Models.Servicos
{
    public class JogoService : IJogoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IQLearningService _qLearningService;

        public JogoService(ApplicationDbContext context, IQLearningService qLearningService)
        {
            _context = context;
            _qLearningService = qLearningService;
        }

        public JogoViewModel IniciarNovoJogo()
        {
            var partida = new Partida
            {
                DataInicio = DateTime.UtcNow,
                Resultado = ResultadoPartida.EmAndamento
            };

            _context.Partidas.Add(partida);
            _context.SaveChanges();

            // Usar estado válido e normalizado
            var tabuleiroInicial = "0,0,0,0,0,0,0,0,0";

            // Criar estado tabuleiro inicial
            var estadoTabuleiro = ObterOuCriarEstadoTabuleiro(tabuleiroInicial);

            var historico = new HistoricoPartida
            {
                PartidaId = partida.Id,
                EstadoTabuleiroId = estadoTabuleiro.Id,
                NumeroJogada = 0,
                PosicaoJogada = -1,
                SimboloJogador = '\0'
            };

            _context.HistoricoPartidas.Add(historico);
            _context.SaveChanges();

            return new JogoViewModel
            {
                PartidaId = partida.Id,
                Tabuleiro = tabuleiroInicial.Split(','),
                VezDoJogador = SimboloJogador.X,
                Resultado = ResultadoPartida.EmAndamento,
                Mensagem = "Jogo iniciado. Sua vez!"
            };
        }
        public JogoViewModel FazerJogada(JogadaViewModel jogada)
        {
            var partida = _context.Partidas
                .Include(p => p.Historico)
                .ThenInclude(h => h.EstadoTabuleiro)
                .FirstOrDefault(p => p.Id == jogada.PartidaId);

            if (partida == null)
                throw new JogoException("Partida não encontrada.");

            if (partida.Resultado != ResultadoPartida.EmAndamento)
                throw new JogoException("Partida já finalizada.");

            // Obter estado atual
            var estadoAtual = ObterEstadoAtualTabuleiro(partida);
            var estadoArray = estadoAtual.Split(',');

            // Verificar se a posição está vazia (usando "0" como string)
            if (estadoArray[jogada.Posicao] != "0")
                throw new JogoException("Posição já ocupada.");

            // Verificar se é a vez do jogador correto
            var countJogadas = estadoArray.Count(pos => pos != "0");
            var simboloEsperado = countJogadas % 2 == 0 ? 'X' : 'O';

            if (jogada.Simbolo != simboloEsperado)
                throw new JogoException($"Não é sua vez de jogar. Esperado: {simboloEsperado}");

            // Fazer a jogada
            estadoArray[jogada.Posicao] = jogada.Simbolo == 'X' ? "X" : "O";
            var novoEstadoString = string.Join(",", estadoArray);

            var estadoTabuleiro = ObterOuCriarEstadoTabuleiro(novoEstadoString);

            var historico = new HistoricoPartida
            {
                PartidaId = partida.Id,
                EstadoTabuleiroId = estadoTabuleiro.Id,
                NumeroJogada = partida.Historico.Count,
                PosicaoJogada = jogada.Posicao,
                SimboloJogador = jogada.Simbolo
            };

            _context.HistoricoPartidas.Add(historico);

            // Verificar resultado
            var resultado = VerificarResultado(novoEstadoString, jogada.Simbolo);
            if (resultado != ResultadoPartida.EmAndamento)
            {
                partida.Resultado = resultado;
                partida.JogadorVencedor = resultado == ResultadoPartida.VitoriaHumano ? "Humano" :
                                         resultado == ResultadoPartida.VitoriaIA ? "IA" : null;
                partida.DataFim = DateTime.UtcNow;
            }

            _context.SaveChanges();

            return ObterEstadoJogo(partida.Id);
        }
        public JogoViewModel FazerJogadaIA(int partidaId)
        {
            var partida = _context.Partidas
                .Include(p => p.Historico)
                .ThenInclude(h => h.EstadoTabuleiro)
                .FirstOrDefault(p => p.Id == partidaId);

            if (partida == null)
                throw new JogoException("Partida não encontrada.");

            if (partida.Resultado != ResultadoPartida.EmAndamento)
                throw new JogoException("Partida já finalizada.");

            // Verificar se é a vez da IA
            var estadoAtual = ObterEstadoAtualTabuleiro(partida);
            var simboloAtual = estadoAtual.Count(c => c == 'X' || c == 'O') % 2 == 0 ? 'X' : 'O';
            var simboloIA = simboloAtual;

            // Obter melhor jogada da IA
            var estadoAtualQL = ConverterParaFormatoQLearning(estadoAtual);
            var posicaoJogada = _qLearningService.ObterMelhorJogada(estadoAtualQL, simboloIA);

            // Registrar jogada
            var estadoArray = estadoAtual.Split(',');
            estadoArray[posicaoJogada] = simboloIA.ToString();
            var novoEstadoString = string.Join(",", estadoArray);

            var estadoTabuleiro = ObterOuCriarEstadoTabuleiro(novoEstadoString);

            var historico = new HistoricoPartida
            {
                PartidaId = partida.Id,
                EstadoTabuleiroId = estadoTabuleiro.Id,
                NumeroJogada = partida.Historico.Count + 1,
                PosicaoJogada = posicaoJogada,
                SimboloJogador = simboloIA
            };

            _context.HistoricoPartidas.Add(historico);
            _context.SaveChanges();

            // Verificar resultado
            var resultado = VerificarResultado(novoEstadoString, simboloIA);
            if (resultado != ResultadoPartida.EmAndamento)
            {
                partida.Resultado = resultado;
                partida.JogadorVencedor = resultado == ResultadoPartida.VitoriaHumano ? "Humano" :
                                          resultado == ResultadoPartida.VitoriaIA ? "IA" : null;
                partida.DataFim = DateTime.UtcNow;
                _context.SaveChanges();

                // Atualizar Q-values
                AtualizarRecompensasPartida(partida.Id, resultado);
            }

            return ObterEstadoJogo(partida.Id);
        }

        public void FinalizarPartida(int partidaId, ResultadoPartida resultado)
        {
            var partida = _context.Partidas.Find(partidaId);
            if (partida == null)
                throw new JogoException("Partida não encontrada.");

            partida.Resultado = resultado;
            partida.DataFim = DateTime.UtcNow;
            _context.SaveChanges();
        }

        public TabuleiroViewModel ObterTabuleiro(int partidaId)
        {
            var partida = _context.Partidas
                .Include(p => p.Historico)
                .ThenInclude(h => h.EstadoTabuleiro)
                .FirstOrDefault(p => p.Id == partidaId);

            if (partida == null)
                throw new JogoException("Partida não encontrada.");

            string ultimoEstado;

            if (!partida.Historico.Any())
            {
                // Estado inicial
                ultimoEstado = "0,0,0,0,0,0,0,0,0";
            }
            else
            {
                ultimoEstado = partida.Historico
                    .OrderByDescending(h => h.NumeroJogada)
                    .FirstOrDefault()?.EstadoTabuleiro?.Estado;
            }

            // Fazer split corretamente e garantir 9 posições
            var tabuleiroArray = ultimoEstado?.Split(',') ?? new string[9];

            // Garantir que sempre temos 9 posições
            if (tabuleiroArray.Length != 9)
            {
                tabuleiroArray = new string[9];
                Array.Fill(tabuleiroArray, "0");
            }

            return new TabuleiroViewModel
            {
                PartidaId = partida.Id,
                Posicoes = tabuleiroArray,
                Resultado = partida.Resultado,
                VezDoJogador = partida.Historico.Count % 2 == 0 ? SimboloJogador.X : SimboloJogador.O
            };
        }

        private string ObterEstadoAtualTabuleiro(Partida partida)
        {
            if (!partida.Historico.Any())
                return "0,0,0,0,0,0,0,0,0";

            var ultimoEstado = partida.Historico
                .OrderByDescending(h => h.NumeroJogada)
                .First().EstadoTabuleiro.Estado;

            return ultimoEstado;
        }

        private EstadoTabuleiro ObterOuCriarEstadoTabuleiro(string estado)
        {
            // Validar e normalizar o estado antes de salvar
            var estadoValidado = ValidarEstado(estado);
            var estadoNormalizado = _qLearningService.NormalizarEstado(estadoValidado);

            var estadoTabuleiro = _context.EstadosTabuleiro
                .FirstOrDefault(e => e.Estado == estadoNormalizado);

            if (estadoTabuleiro == null)
            {
                estadoTabuleiro = new EstadoTabuleiro
                {
                    Estado = estadoNormalizado,
                    Vitoria = false,
                    Derrota = false,
                    Empate = false,
                    ValorQ = 0,
                    DataCriacao = DateTime.UtcNow,
                    DataAtualizacao = DateTime.UtcNow
                };

                _context.EstadosTabuleiro.Add(estadoTabuleiro);
                _context.SaveChanges();
            }

            return estadoTabuleiro;
        }

        private string ValidarEstado(string estado)
        {
            // Garantir que o estado tem o formato correto
            if (string.IsNullOrEmpty(estado))
                return "0,0,0,0,0,0,0,0,0";

            // Se já está no formato com vírgulas, validar
            if (estado.Contains(','))
            {
                var partes = estado.Split(',');
                if (partes.Length != 9)
                    return "0,0,0,0,0,0,0,0,0";

                return estado;
            }

            // Se está no formato sem vírgulas, converter
            if (estado.Length == 9)
            {
                var array = estado.ToCharArray()
                    .Select(c => c == ' ' || c == '\0' ? "0" : c.ToString())
                    .ToArray();
                return string.Join(",", array);
            }

            return "0,0,0,0,0,0,0,0,0";
        }
        private ResultadoPartida VerificarResultado(string estado, char simbolo)
        {
            // Primeiro, remover as vírgulas para ter uma string contínua
            var estadoLimpo = estado.Replace(",", "");

            // Verificar linhas
            for (int i = 0; i < 9; i += 3)
            {
                if (estadoLimpo[i] != '0' && estadoLimpo[i] == estadoLimpo[i + 1] && estadoLimpo[i] == estadoLimpo[i + 2])
                {
                    return estadoLimpo[i] == 'X' ? ResultadoPartida.VitoriaHumano : ResultadoPartida.VitoriaIA;
                }
            }

            // Verificar colunas
            for (int i = 0; i < 3; i++)
            {
                if (estadoLimpo[i] != '0' && estadoLimpo[i] == estadoLimpo[i + 3] && estadoLimpo[i] == estadoLimpo[i + 6])
                {
                    return estadoLimpo[i] == 'X' ? ResultadoPartida.VitoriaHumano : ResultadoPartida.VitoriaIA;
                }
            }

            // Verificar diagonais
            if (estadoLimpo[0] != '0' && estadoLimpo[0] == estadoLimpo[4] && estadoLimpo[0] == estadoLimpo[8])
            {
                return estadoLimpo[0] == 'X' ? ResultadoPartida.VitoriaHumano : ResultadoPartida.VitoriaIA;
            }

            if (estadoLimpo[2] != '0' && estadoLimpo[2] == estadoLimpo[4] && estadoLimpo[2] == estadoLimpo[6])
            {
                return estadoLimpo[2] == 'X' ? ResultadoPartida.VitoriaHumano : ResultadoPartida.VitoriaIA;
            }

            // Verificar empate - verifica se não há mais "0" (posições vazias)
            if (!estadoLimpo.Contains('0'))
            {
                return ResultadoPartida.Empate;
            }

            return ResultadoPartida.EmAndamento;
        }
        private JogoViewModel ObterEstadoJogo(int partidaId)
        {
            var tabuleiro = ObterTabuleiro(partidaId);
            var partida = _context.Partidas.Find(partidaId);

            return new JogoViewModel
            {
                PartidaId = partidaId,
                Tabuleiro = tabuleiro.Posicoes,
                VezDoJogador = tabuleiro.VezDoJogador,
                Resultado = partida.Resultado,
                Mensagem = partida.Resultado == ResultadoPartida.EmAndamento ?
                          (tabuleiro.VezDoJogador == SimboloJogador.X ? "Sua vez (X)!" : "Vez da IA (O)") :
                          partida.Resultado == ResultadoPartida.VitoriaHumano ? "Você venceu!" :
                          partida.Resultado == ResultadoPartida.VitoriaIA ? "IA venceu!" : "Empate!"
            };
        }

        private void AtualizarRecompensasPartida(int partidaId, ResultadoPartida resultado)
        {
            var historico = _context.HistoricoPartidas
                .Include(h => h.EstadoTabuleiro)
                .Where(h => h.PartidaId == partidaId)
                .OrderBy(h => h.NumeroJogada)
                .ToList();

            for (int i = 0; i < historico.Count; i++)
            {
                var estadoAtual = historico[i].EstadoTabuleiro.Estado;
                var acao = historico[i].PosicaoJogada;
                var simbolo = historico[i].SimboloJogador;

                string proximoEstado = i < historico.Count - 1 ? historico[i + 1].EstadoTabuleiro.Estado : estadoAtual;

                double recompensa = 0;
                if (i == historico.Count - 1) // Última jogada
                {
                    recompensa = resultado == ResultadoPartida.VitoriaIA ? 1 :
                                resultado == ResultadoPartida.VitoriaHumano ? -1 : 0;
                }

                var estadoAtualQL = ConverterParaFormatoQLearning(estadoAtual);
                var proximoEstadoQL = ConverterParaFormatoQLearning(proximoEstado);
                _qLearningService.AtualizarQValue(estadoAtualQL, acao, proximoEstadoQL, recompensa);
            }
        }
        private char ObterSimboloDaVez(string estado)
        {
            var array = estado.Split(',');
            var countX = array.Count(pos => pos == "X");
            var countO = array.Count(pos => pos == "O");

            return countX <= countO ? 'X' : 'O';
        }
        private string ConverterParaFormatoQLearning(string estadoComVirgulas)
        {
            // Converte "0,0,0,X,0,0,0,0,0" para "   X    " (espaços para posições vazias)
            var array = estadoComVirgulas.Split(',');
            return string.Join("", array.Select(s => s == "0" ? ' ' : s[0]));
        }

        private string ConverterParaFormatoJogo(string estadoSemVirgulas)
        {
            // Converte "   X    " para "0,0,0,X,0,0,0,0,0"
            var array = estadoSemVirgulas.ToCharArray()
                .Select(c => c == ' ' ? "0" : c.ToString())
                .ToArray();
            return string.Join(",", array);
        }
    }
}