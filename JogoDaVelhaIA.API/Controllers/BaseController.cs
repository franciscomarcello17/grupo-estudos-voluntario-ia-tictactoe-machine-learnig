using Microsoft.AspNetCore.Mvc;
using JogoDaVelhaIA.DTOs;
using JogoDaVelhaIA.Interfaces;

namespace JogoDaVelhaIA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private readonly IJogadaService _jogadaService;

        public BaseController(IJogadaService jogadaService)
        {
            _jogadaService = jogadaService;
        }

        [HttpGet("jogadas-base")]
        public IActionResult Get()
        {
            var jogadas = _jogadaService.ObterTodasJogadasBase();
            return Ok(jogadas);
        }

        [HttpPost("jogar-com-base")]
        public IActionResult Post([FromBody] JogadaRequest jogada)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resposta = _jogadaService.ObterMelhorJogadaBase(jogada);
            return Ok(resposta);
        }
    }
}