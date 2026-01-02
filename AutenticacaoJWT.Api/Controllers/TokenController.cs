using AutenticacaoJWT.Api.Configuracao;
using AutenticacaoJWT.Aplicacao.Controller.Token.GerarToken;
using AutenticacaoJWT.Aplicacao.Controller.Token.RefreshToken;
using AutenticacaoJWT.Dominio.Entidade;
using AutenticacaoJWT.Dominio.InterfaceRepositorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AutenticacaoJWT.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {



        public readonly IUsuarioRepositorio _IUsuarioRepositorio;
        private readonly GerartokenHandler _gerartokenHandler;
        private readonly RefreshTokenHandler _refreshTokenHandler;

        public TokenController(IUsuarioRepositorio iUsuarioRepositorio, GerartokenHandler gerartokenHandler, RefreshTokenHandler refreshTokenHandler)
        {
            _gerartokenHandler = gerartokenHandler;
            _refreshTokenHandler = refreshTokenHandler;
            _IUsuarioRepositorio = iUsuarioRepositorio;
        }

       // [LogController]
        [AllowAnonymous]
        [HttpPost("GerarToken")]
        public async Task<IActionResult> GerarToken([FromQuery] GerarTokenRequest loginRequest, CancellationToken cancellationToken)
        {
            GerarTokenResponse response = await _gerartokenHandler.GerarToken(loginRequest, cancellationToken);
            return Ok(new { response });
        }




        // =====================================================
        // ROTA PÚBLICA (SEM TOKEN)
        // =====================================================
        [AllowAnonymous]
        [HttpGet("publica")]
        public IActionResult Publica()
        {
            return Ok(new
            {
                Sucesso = true,
                Mensagem = "Rota pública acessada com sucesso (sem token)"
            });
        }

        // =====================================================
        // ROTA PROTEGIDA (COM TOKEN)
        // =====================================================
        [Authorize]
        [HttpGet("protegida")]
        public IActionResult Protegida()
        {
            return Ok(new
            {
                Sucesso = true,
                Mensagem = "Token válido"
            });
        }


    }

}

