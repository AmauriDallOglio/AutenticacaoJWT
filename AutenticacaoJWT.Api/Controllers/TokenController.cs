using AutenticacaoJWT.Aplicacao.IServico;
using AutenticacaoJWT.Aplicacao.Request;
using AutenticacaoJWT.Dominio.Entidade;
using AutenticacaoJWT.Dominio.InterfaceRepositorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutenticacaoJWT.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {

 
        private readonly IUsuarioRepositorio _IUsuarioRepositorio;
        private readonly ITokenServico _ITokenServico;
        private readonly ITokenConfiguracaoServico _iTokenConfiguracaoServico;
        private static readonly Dictionary<string, string> _tokenUsuario = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> _codigoUsuario = new Dictionary<string, string>();


        public TokenController( IUsuarioRepositorio usuarioRepositorio, ITokenServico iTokenServico, ITokenConfiguracaoServico iTokenConfiguracaoServico)
        {
            _ITokenServico = iTokenServico;
            _IUsuarioRepositorio = usuarioRepositorio;
            _iTokenConfiguracaoServico = iTokenConfiguracaoServico;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Aplicacao.Request.LoginRequest loginRequest, CancellationToken cancellationToken)
        {
            Usuario usuario = await _ITokenServico.GerarToken(loginRequest, cancellationToken);
            return Ok(new { usuario });
        }


        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        public IActionResult RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            Usuario usuario = _ITokenServico.GerarRefreshToken(tokenRequest);


            return Ok(new { AccessToken = usuario.Token, RefreshToken = usuario.Refresh });
        }
    }
}
