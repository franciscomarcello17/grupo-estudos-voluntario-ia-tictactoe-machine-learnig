using JogoDaVelhIA.API.Models.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace JogoDaVelhIA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TreinamentoController : ControllerBase
    {
        private readonly IQLearningService _qLearningService;

        public TreinamentoController(IQLearningService qLearningService)
        {
            _qLearningService = qLearningService;
        }

        [HttpPost("iniciar")]
        public async Task<IActionResult> IniciarTreinamento([FromQuery] int episodios = 1000)
        {
            try
            {
                await _qLearningService.TreinarIAAsync(episodios);
                return Ok(new { Mensagem = $"Treinamento concluído com {episodios} episódios." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = ex.Message });
            }
        }

        [HttpPost("parametros")]
        public IActionResult DefinirParametros([FromQuery] double taxaAprendizado = 0.1,
                                              [FromQuery] double fatorDesconto = 0.9,
                                              [FromQuery] double epsilon = 0.3)
        {
            try
            {
                _qLearningService.InicializarParametros(taxaAprendizado, fatorDesconto, epsilon);
                return Ok(new
                {
                    Mensagem = "Parâmetros atualizados",
                    TaxaAprendizado = taxaAprendizado,
                    FatorDesconto = fatorDesconto,
                    Epsilon = epsilon
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = ex.Message });
            }
        }
    }
}