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

            return new JogoViewModel
            {
                PartidaId = partida.Id,
                Tabuleiro = new string[9],
                VezDoJogador = SimboloJogador.X,
                Resultado = ResultadoPartida.EmAndamento,
                Mensagem = "Jogo iniciado. Sua vez!"
            };
        }

        public JogoViewModel FazerJogada(JogadaViewModel jogada)
        {
            var partida = _context.Partidas
                .Include(p => p.Historico)
                .FirstOrDefault(p => p.Id == jogada.PartidaId);

            if (partida == null)
                throw new JogoException("Partida não encontrada.");

            if (partida.Resultado != ResultadoPartida.EmAndamento)
                throw new JogoException("Partida já finalizada.");

            // Verificar se é a vez do jogador humano
            var estadoAtual = ObterEstadoAtualTabuleiro(partida);
            var simboloAtual = estadoAtual.Count(c => c == 'X' || c == 'O') % 2 == 0 ? 'X' : 'O';

            if (simboloAtual != jogada.Simbolo)
                throw new JogoException("Não é sua vez de jogar.");

            // Validar jogada
            if (estadoAtual[jogada.Posicao] != '\0')
                throw new JogoException("Posição já ocupada.");

            // Registrar jogada
            var novoEstado = estadoAtual.ToCharArray();
            novoEstado[jogada.Posicao] = jogada.Simbolo;
            var novoEstadoString = new string(novoEstado);

            var estadoTabuleiro = ObterOuCriarEstadoTabuleiro(novoEstadoString);

            var historico = new HistoricoPartida
            {
                PartidaId = partida.Id,
                EstadoTabuleiroId = estadoTabuleiro.Id,
                NumeroJogada = partida.Historico.Count + 1,
                PosicaoJogada = jogada.Posicao,
                SimboloJogador = jogada.Simbolo
            };

            _context.HistoricoPartidas.Add(historico);
            _context.SaveChanges();

            // Verificar resultado
            var resultado = VerificarResultado(novoEstadoString, jogada.Simbolo);
            if (resultado != ResultadoPartida.EmAndamento)
            {
                partida.Resultado = resultado;
                partida.JogadorVencedor = resultado == ResultadoPartida.VitoriaHumano ? "Humano" :
                                         resultado == ResultadoPartida.VitoriaIA ? "IA" : null;
                partida.DataFim = DateTime.UtcNow;
                _context.SaveChanges();

                // Atualizar Q-values se a IA participou
                if (resultado != ResultadoPartida.Empate)
                {
                    AtualizarRecompensasPartida(partida.Id, resultado);
                }
            }

            return ObterEstadoJogo(partida.Id);
        }

        public JogoViewModel FazerJogadaIA(int partidaId)
        {
            var partida = _context.Partidas
                .Include(p => p.Historico)
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
            var posicaoJogada = _qLearningService.ObterMelhorJogada(estadoAtual, simboloIA);

            // Registrar jogada
            var novoEstado = estadoAtual.ToCharArray();
            novoEstado[posicaoJogada] = simboloIA;
            var novoEstadoString = new string(novoEstado);

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

            var ultimoEstado = partida.Historico
                .OrderByDescending(h => h.NumeroJogada)
                .FirstOrDefault()?.EstadoTabuleiro?.Estado;

            var tabuleiro = ultimoEstado?.Split(',') ?? new string[9];

            return new TabuleiroViewModel
            {
                PartidaId = partida.Id,
                Posicoes = tabuleiro,
                Resultado = partida.Resultado,
                VezDoJogador = partida.Historico.Count % 2 == 0 ? SimboloJogador.X : SimboloJogador.O
            };
        }

        private string ObterEstadoAtualTabuleiro(Partida partida)
        {
            if (!partida.Historico.Any())
                return new string('\0', 9);

            var ultimoEstado = partida.Historico
                .OrderByDescending(h => h.NumeroJogada)
                .First().EstadoTabuleiro.Estado;

            return ultimoEstado;
        }

        private EstadoTabuleiro ObterOuCriarEstadoTabuleiro(string estado)
        {
            var estadoNormalizado = _qLearningService.NormalizarEstado(estado);

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

        private ResultadoPartida VerificarResultado(string estado, char simbolo)
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

                _qLearningService.AtualizarQValue(estadoAtual, acao, proximoEstado, recompensa);
            }
        }
    }
}