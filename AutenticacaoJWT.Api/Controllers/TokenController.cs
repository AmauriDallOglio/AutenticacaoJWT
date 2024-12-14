using AutenticacaoJWT.Api.Configuracao;
using AutenticacaoJWT.Aplicacao.Dto;
using AutenticacaoJWT.Aplicacao.DTO;
using AutenticacaoJWT.Aplicacao.ServicoInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutenticacaoJWT.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {

        private readonly IUsuarioServico _usuarioServico;
        private static readonly Dictionary<string, string> _refreshTokens = new Dictionary<string, string>();


        public TokenController( IUsuarioServico usuarioServico)
        {
            _usuarioServico = usuarioServico;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginDTO loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Email))
                throw new ArgumentException("Email não pode ser vazio ou nulo.", nameof(loginRequest.Email));

            if (string.IsNullOrWhiteSpace(loginRequest.Senha))
                throw new ArgumentException("Senha não pode ser vazio ou nulo.", nameof(loginRequest.Senha));


            // Valida as credenciais
            if (!_usuarioServico.ValidarCredenciais(loginRequest.Email, loginRequest.Senha))
            {
                return Unauthorized(new { Message = "Credenciais inválidas" });
            }

            // Gera o token JWT
            var token = new SwaggerConfigracao().GerarJwtToken(loginRequest.Email);
            _refreshTokens[loginRequest.Email] = token;

            return Ok(new { Token = token });
        }


        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        public IActionResult RefreshToken([FromBody] RefreshTokenDTO refreshTokenDTO)
        {
            // Verifica se o refresh token é válido
            var userEmail = _refreshTokens.FirstOrDefault(x => x.Value == refreshTokenDTO.Token).Key;

            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new { Message = "Refresh token inválido ou expirado" });
            }

            // Gera um novo access token
            var newAccessToken = new SwaggerConfigracao().GerarJwtToken(userEmail);
            var newRefreshToken = new SwaggerConfigracao().GerarRefreshToken();

            // Atualiza o refresh token armazenado
            _refreshTokens[userEmail] = newRefreshToken;

            return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
        }

    }

}
