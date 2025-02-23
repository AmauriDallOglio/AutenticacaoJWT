using AutenticacaoJWT.Api.Configuracao;
using AutenticacaoJWT.Aplicacao.Request;
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
        private static readonly Dictionary<string, string> _tokenUsuario = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> _codigoUsuario = new Dictionary<string, string>();


        public TokenController( IUsuarioRepositorio usuarioRepositorio)
        {
 
            _IUsuarioRepositorio = usuarioRepositorio;

        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Email))
                throw new ArgumentException("E-mail não informado ", nameof(loginRequest.Email));

            if (string.IsNullOrWhiteSpace(loginRequest.Senha))
                throw new ArgumentException("Senha não informada", nameof(loginRequest.Senha));


            var usuario = _IUsuarioRepositorio.ObterUsuarioPorEmail(loginRequest.Email, loginRequest.Senha);

            if (usuario is null)
            {
                return Unauthorized(new { Message = "Usuário inválido" });
            }
            else
            {
                if(usuario.Equals(loginRequest.Senha))
                {
                    return Unauthorized(new { Message = "Acesso não permitido" });
                }
            }

            var token = new TokenConfiguracao().GerarJwtToken(loginRequest.Email);
            var codigo = new TokenConfiguracao().GerarRefreshToken();

            usuario.UltimoAcesso = DateTime.Now;
            usuario.Token = token;
            usuario.Codigo = codigo;

            return Ok(new { usuario });
        }


        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        public IActionResult RefreshToken([FromBody] TokenRequest refreshTokenDTO)
        {
            var email = _tokenUsuario.FirstOrDefault(x => x.Value == refreshTokenDTO.Token).Key;
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized(new { Message = "Token inválido!" });
            }

            var token = new TokenConfiguracao().GerarJwtToken(email);
            var codigo = new TokenConfiguracao().GerarRefreshToken();

            _tokenUsuario[email] = token; 
            _codigoUsuario[email] = codigo;

            return Ok(new { AccessToken = token, RefreshToken = codigo });
        }
    }
}
