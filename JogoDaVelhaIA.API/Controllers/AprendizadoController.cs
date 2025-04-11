using Microsoft.AspNetCore.Mvc;
using JogoDaVelhaIA.DTOs;
using JogoDaVelhaIA.Interfaces;

namespace JogoDaVelhaIA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AprendizadoController : ControllerBase
    {
        private readonly IJogadaService _jogadaService;

        public AprendizadoController(IJogadaService jogadaService)
        {
            _jogadaService = jogadaService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var jogadas = _jogadaService.ObterTodasJogadasAprendidas();
            return Ok(jogadas);
        }

        [HttpPost("treinar")]
        public IActionResult Post([FromBody] JogadaRequest jogada)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _jogadaService.AdicionarJogadaAprendida(jogada);
            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            _jogadaService.LimparJogadasAprendidas();
            return NoContent();
        }
    }
}