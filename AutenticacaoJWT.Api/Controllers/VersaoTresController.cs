using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutenticacaoJWT.Api.Controllers
{
    [ApiController]
    [ApiVersion("3.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Authorize(Policy = "AcessoV3")] // Apenas quem tem claim AcessoApi=v3
    public class VersaoTresController : ControllerBase
    {
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
