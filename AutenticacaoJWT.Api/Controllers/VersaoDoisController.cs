using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutenticacaoJWT.Api.Controllers
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Authorize(Policy = "AcessoV2")] // Apenas quem tem claim AcessoApi=v3
    public class VersaoDoisController : ControllerBase
    {
        public IActionResult Index()
        {
            return Ok();
        }

        [HttpGet("PingRespostaV2")]
        public IActionResult PingRespostaV2()
        {

            return Ok(new { Sucesso = true, Mensagem = "Resposta v2", Codigo = 200, Tempo = DateTime.UtcNow });

        }
    }
}
