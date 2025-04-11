using JogoDaVelhaIA.Models;
using JogoDaVelhaIA.Interfaces;
using Microsoft.EntityFrameworkCore;
using JogoDaVelhaIA.Data;

namespace JogoDaVelhaIA.Repositories
{
    public class JogadaRepository : IJogadaRepository
    {
        private readonly AppDbContext _context;

        public JogadaRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<JogadaAprendida> GetAllAprendidas()
        {
            return _context.JogadasAprendidas.ToList();
        }

        public void AddAprendida(JogadaAprendida jogada)
        {
            _context.JogadasAprendidas.Add(jogada);
            _context.SaveChanges();
        }

        public void ClearAprendidas()
        {
            _context.JogadasAprendidas.RemoveRange(_context.JogadasAprendidas);
            _context.SaveChanges();
        }

        public IEnumerable<JogadaBase> GetAllBase()
        {
            return _context.JogadasBase.ToList();
        }

        public int GetBestMoveFromBase(string estado)
        {
            var jogadas = _context.JogadasBase
                .Where(j => j.EstadoTabuleiro == estado)
                .OrderByDescending(j => j.Peso)
                .ToList();

            if (jogadas.Any())
            {
                // Retorna a jogada com maior peso
                return jogadas.First().PosicaoEscolhida;
            }

            // Se não encontrar jogada, retorna -1 (deve ser tratado pelo serviço)
            return -1;
        }
    }
}