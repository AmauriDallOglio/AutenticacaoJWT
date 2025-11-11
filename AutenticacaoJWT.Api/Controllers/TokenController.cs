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

        [LogController]
        [AllowAnonymous]
        [HttpPost("GerarToken")]
        public async Task<IActionResult> GerarToken([FromQuery] GerarTokenRequest loginRequest, CancellationToken cancellationToken)
        {
            //GerarTokenResponse response = await _gerartokenHandler.GerarToken(loginRequest, cancellationToken);
            //return Ok(new { response });

            if (string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Senha))
                return BadRequest(new { Codigo = 400, Mensagem = "Login e senha são obrigatórios." });

            Usuario? usuario = await _IUsuarioRepositorio.ObterUsuarioPorEmailSenhaAsync(loginRequest.Email, loginRequest.Senha);
            if (usuario == null)
                return Unauthorized(new { mensagem = "Usuário não encontrado!" });

            // Chave secreta usada para assinar o token
            var key = Encoding.UTF8.GetBytes("minha_chave_secreta_super_segura");

            // Criação da lista de claims
            var claims = new List<Claim>
            {
                new Claim("Id", usuario.Id.ToString()),
                new Claim("Email", usuario.Email),
                new Claim("Nome", usuario.Nome),
                new Claim("Refresh", usuario.Refresh ?? ""),
                new Claim("Codigo", usuario.Codigo ?? ""),
                new Claim("Aplicativo", usuario.Aplicativo ?? ""),
                new Claim("DataCadastro", usuario.DataCadastro.ToString("o")), // formato ISO 8601
                new Claim("UltimoAcesso", usuario.UltimoAcesso?.ToString("o") ?? ""),
                new Claim("Permissions", "read,write") // exemplo de permissão
            };


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(60), // O token expira em 60 minutos a partir de agora (UTC)
                NotBefore = DateTime.Now, // Define quando o token começa a valer (agora, em UTC)
                IssuedAt = DateTime.Now, // Define quando o token foi emitido (agora, em UTC)

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(securityToken);

            // Retorna token e data de expiração convertida para hora local
            return Ok(new
            {
                token = tokenString,
                expiracao = securityToken.ValidTo.ToLocalTime() // Mostra em hora local
            });
        }





        [LogController]
        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromQuery] RefreshTokenRequest refreshRequest, CancellationToken cancellationToken)
        {
            RefreshTokenResponse refreshResponse = _refreshTokenHandler.GerarRefresh(refreshRequest, cancellationToken);
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
