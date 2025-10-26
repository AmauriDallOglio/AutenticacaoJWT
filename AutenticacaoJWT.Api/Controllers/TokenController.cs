using AutenticacaoJWT.Api.Configuracao;
using AutenticacaoJWT.Aplicacao.Controller.Token.GerarToken;
using AutenticacaoJWT.Aplicacao.Controller.Token.RefreshToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutenticacaoJWT.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {




        private readonly GerartokenHandler _gerartokenHandler;
        private readonly RefreshTokenHandler _refreshTokenHandler;

        public TokenController(GerartokenHandler gerartokenHandler, RefreshTokenHandler refreshTokenHandler)
        {


            _gerartokenHandler = gerartokenHandler;
            _refreshTokenHandler = refreshTokenHandler;
        }

        [LogController]
        [AllowAnonymous]
        [HttpPost("GerarToken")]
        public async Task<IActionResult> GerarToken([FromBody] GerarTokenRequest loginRequest, CancellationToken cancellationToken)
        {
            GerarTokenResponse response = await _gerartokenHandler.GerarToken(loginRequest, cancellationToken);
            return Ok(new { response });
        }





        [LogController]
        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshRequest)
        {
            RefreshTokenResponse refreshResponse = _refreshTokenHandler.GerarRefresh(refreshRequest);
            return Ok(refreshResponse);
        }



        //[ApiVersion("1.0")]
        //[Route("api/v{version:apiVersion}/[controller]")]
        //[Authorize(Policy = "AcessoV1")] // Requer claim AcessoApi=v3
        //[HttpGet("ObterPerfilAcessoV1")]
        //public IActionResult ObterPerfilAcessoV1()
        //{
        //    return Ok(new
        //    {
        //        sucesso = true,
        //        versao = "v1.0",
        //        mensagem = "Acesso autorizado à versão 1 da API.",
        //        usuario = User.Identity?.Name,
        //        claims = User.Claims.Select(c => new { c.Type, c.Value })
        //    });
        //}


        //[ApiVersion("2.0")]
        //[Route("api/v{version:apiVersion}/[controller]")]
        //[Authorize(Policy = "AcessoV2")] // Requer claim AcessoApi=v3
        //[HttpGet("ObterPerfilAcessoV2")]
        //public IActionResult ObterPerfilAcessoV2()
        //{
        //    return Ok(new
        //    {
        //        sucesso = true,
        //        versao = "v2.0",
        //        mensagem = "Acesso autorizado à versão 2 da API.",
        //        usuario = User.Identity?.Name,
        //        claims = User.Claims.Select(c => new { c.Type, c.Value })
        //    });
        //}


        //[ApiVersion("3.0")]
        //[Route("api/v{version:apiVersion}/[controller]")]
        //[Authorize(Policy = "AcessoV3")] // Requer claim AcessoApi=v3
        //[HttpGet("ObterPerfilAcessoV3")]
        //public IActionResult ObterPerfilAcessoV3()
        //{
        //    return Ok(new
        //    {
        //        sucesso = true,
        //        versao = "v3.0",
        //        mensagem = "Acesso autorizado à versão 3 da API.",
        //        usuario = User.Identity?.Name,
        //        claims = User.Claims.Select(c => new { c.Type, c.Value })
        //    });
        //}

    }
}
