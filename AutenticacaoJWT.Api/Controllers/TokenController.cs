using AutenticacaoJWT.Aplicacao.IServico;
using AutenticacaoJWT.Aplicacao.Request;
using AutenticacaoJWT.Aplicacao.Response;
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
        private readonly IRefreshServico _IRefreshServico;
        private readonly ITokenConfiguracaoServico _iTokenConfiguracaoServico;
        private static readonly Dictionary<string, string> _tokenUsuario = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> _codigoUsuario = new Dictionary<string, string>();


        public TokenController( IUsuarioRepositorio usuarioRepositorio, ITokenServico iTokenServico, IRefreshServico iRefreshServico ,ITokenConfiguracaoServico iTokenConfiguracaoServico)
        {
            _ITokenServico = iTokenServico;
            _IUsuarioRepositorio = usuarioRepositorio;
            _iTokenConfiguracaoServico = iTokenConfiguracaoServico;
            _IRefreshServico = iRefreshServico;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Aplicacao.Request.LoginRequest loginRequest, CancellationToken cancellationToken)
        {
            TokenResponse response = await _ITokenServico.GerarToken(loginRequest, cancellationToken);
            return Ok(new { response });
        }


        [AllowAnonymous]
        [HttpPost("Refresh")]
        public IActionResult RefreshToken([FromBody] RefreshRequest refreshRequest)
        {
            RefreshResponse refreshResponse = _IRefreshServico.GerarRefresh(refreshRequest);


            return Ok(refreshResponse);
        }
    }
}
