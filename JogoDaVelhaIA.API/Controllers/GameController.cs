using JogoDaVelhIA.Data;
using JogoDaVelhIA.Models;
using JogoDaVelhIA.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace JogoDaVelhIA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly ApplicationDbContext _context; 
        public GameController(IGameService gameService, ApplicationDbContext context)
        {
            _gameService = gameService;
            _context = context;
        }

        [HttpPost("new")]
        public async Task<IActionResult> NewGame()
        {
            var gameState = await _gameService.StartNewGame();
            return Ok(gameState);
        }

        [HttpPost("move")]
        public async Task<IActionResult> MakeMove([FromBody] PlayerMoveRequest request)
        {
            var gameState = await _gameService.MakeMove(request.Position, request.CurrentState);
            return Ok(gameState);
        }

        [HttpGet("current-state")]
        public IActionResult GetCurrentState()
        {
            // Para visualizar o estado atual durante os testes
            return Ok(_gameService.GetCurrentState());
        }

        [HttpGet("database-states")]
        public async Task<IActionResult> GetDatabaseStates()
        {
            var states = await _context.GameStates.ToListAsync();
            return Ok(states);
        }

        public class PlayerMoveRequest
        {
            public int Position { get; set; }
            public CurrentGameState CurrentState { get; set; }
        }
    }
}