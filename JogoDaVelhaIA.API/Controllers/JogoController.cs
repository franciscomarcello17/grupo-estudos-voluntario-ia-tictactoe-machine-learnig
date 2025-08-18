using JogoDaVelhaIA.API.Models.ViewModels;
using JogoDaVelhaIA.Models.ViewModels;
using JogoDaVelhIA.API.Models.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace JogoDaVelhIA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JogoController : ControllerBase
    {
        private readonly IJogoService _jogoService;

        public JogoController(IJogoService jogoService)
        {
            _jogoService = jogoService;
        }

        [HttpPost("novo")]
        public IActionResult NovoJogo()
        {
            try
            {
                var jogo = _jogoService.IniciarNovoJogo();
                return Ok(jogo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = ex.Message });
            }
        }

        [HttpPost("jogar")]
        public IActionResult FazerJogada([FromBody] JogadaViewModel jogada)
        {
            try
            {
                var jogo = _jogoService.FazerJogada(jogada);
                return Ok(jogo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = ex.Message });
            }
        }

        [HttpPost("jogar-ia/{partidaId}")]
        public IActionResult FazerJogadaIA(int partidaId)
        {
            try
            {
                var jogo = _jogoService.FazerJogadaIA(partidaId);
                return Ok(jogo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = ex.Message });
            }
        }

        [HttpGet("tabuleiro/{partidaId}")]
        public IActionResult ObterTabuleiro(int partidaId)
        {
            try
            {
                var tabuleiro = _jogoService.ObterTabuleiro(partidaId);
                return Ok(tabuleiro);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = ex.Message });
            }
        }
    }
}