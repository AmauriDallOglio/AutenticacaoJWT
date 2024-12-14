using AutenticacaoJWT.Aplicacao.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AutenticacaoJWT.Api.Controllers
{
    public class SsoAuthController : ControllerBase
    {
        private readonly string _jwtSecret = "SuperSecretKey@1234567890123456abcdefghijlmn";
        private readonly string _issuer = "AuthServer";
        private readonly string _audience = "YourAudience";

        // Autenticar e gerar o JWT
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginDTO loginRequest)
        {
            // Validar as credenciais do usuário com um banco de dados ou serviço de autenticação centralizado
            if (loginRequest.Email == "amauri1@amauri.com" && loginRequest.Senha == "amauri123")
            {
                var token = GenerateJwtToken(loginRequest.Email, "UserName", 12345); // Informações do usuário
                return Ok(new { Token = token });
            }

            return Unauthorized(new { Message = "Credenciais inválidas" });
        }

        private string GenerateJwtToken(string email, string userName, int userId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, "User")
        };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
