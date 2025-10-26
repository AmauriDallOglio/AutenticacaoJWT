using AutenticacaoJWT.Aplicacao.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AutenticacaoJWT.Api.Controllers
{
 
    [ApiController]
    [Route("api/[controller]")]
    public class VersaoUmController : ControllerBase
    {
        [HttpPost]
        public IActionResult GerarTokenDto(string login, string senha)
        {
            if (login == null || senha == null)
                return Unauthorized(new { mensagem = "Usuário ou senha inválidos" });

  
            // Busca usuário na lista estática
            UsuarioDto usuario = UsuarioDto.ListaUsuariosDto().FirstOrDefault(u =>
                u.Nome.Equals(login, StringComparison.OrdinalIgnoreCase) &&
                u.Senha == senha);

 

            // Chave secreta usada para assinar o token
            var key = Encoding.UTF8.GetBytes("minha_chave_secreta_super_segura");

            // Criação da lista de claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Nome) // Nome do usuário
            };

            // Adiciona claims de roles (papéis do usuário)
            foreach (var role in usuario.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Adiciona claims de versões de API que o usuário pode acessar
            foreach (var versao in usuario.Versoes)
            {
                claims.Add(new Claim("AcessoApi", versao));
            }


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
    }
}
