using JogoDaVelhaIA.DTOs;
using JogoDaVelhaIA.Interfaces;
using JogoDaVelhaIA.Models;
using JogoDaVelhaIA.Repositories;

namespace JogoDaVelhaIA.Services
{
    public class JogadaService : IJogadaService
    {
        private readonly IJogadaRepository _repository;

        public JogadaService(IJogadaRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<JogadaAprendida> ObterTodasJogadasAprendidas()
        {
            return _repository.GetAllAprendidas();
        }

        public void AdicionarJogadaAprendida(JogadaRequest jogada)
        {
            var novaJogada = new JogadaAprendida
            {
                EstadoTabuleiro = jogada.EstadoTabuleiro,
                PosicaoEscolhida = jogada.PosicaoEscolhida,
                Resultado = jogada.Resultado,
                Data = jogada.Data
            };

            _repository.AddAprendida(novaJogada);
        }

        public void LimparJogadasAprendidas()
        {
            _repository.ClearAprendidas();
        }

        public IEnumerable<JogadaBase> ObterTodasJogadasBase()
        {
            return _repository.GetAllBase();
        }

        public JogadaResponse ObterMelhorJogadaBase(JogadaRequest jogada)
        {
            var posicao = _repository.GetBestMoveFromBase(jogada.EstadoTabuleiro);

            // Se não encontrou jogada na base, escolhe aleatório
            if (posicao == -1)
            {
                posicao = EscolherJogadaAleatoriaValida(jogada.EstadoTabuleiro);
            }

            return new JogadaResponse { PosicaoEscolhida = posicao };
        }

        private int EscolherJogadaAleatoriaValida(string estadoTabuleiro)
        {
            var posicoesVazias = new List<int>();
            var celulas = estadoTabuleiro.Split(',');

            for (int i = 0; i < celulas.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(celulas[i]))
                    posicoesVazias.Add(i);
            }

            if (posicoesVazias.Count == 0)
                return -1;

            var random = new Random();
            return posicoesVazias[random.Next(posicoesVazias.Count)];
        }
    }
}