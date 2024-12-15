using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AutenticacaoJWT.Api.Configuracao
{
    internal sealed class TokenConfigracao
    {
        private readonly string secretKey = "SuperSecretKey@1234567890123456abcdefghijlmn";
        private readonly string _issuer = "JwtInMemoryAuth";
        private readonly string _audience = "JwtInMemoryAuth";
        public byte[] CodigoChave()
        {
            byte[] chave = Encoding.UTF8.GetBytes(secretKey);
            if (chave.Length < 32)
            {
                throw new Exception($"A chave precisa ter pelo menos 32 bytes. Chave atual: {chave.Length} bytes.");
            }
            return chave;
        }

        public string CodigoSecret()
        {
            return secretKey;
        }

        public string GerarJwtToken(string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
 

            var claims = new[]
            {
                new Claim("Email", email),
                new Claim("AppId", "appId"),  // Identifica o aplicativo
                new Claim("Nome", "amauri"),  // Nome do usuário
                new Claim("Id", "123456"),  // ID do usuário
                new Claim("Usuario", "User"),  // Papel do usuário
                new Claim("CustomClaim", "CustomValue"), // Claim personalizada
                new Claim("DateCreated", DateTime.Now.ToString()),  // Data de criação
                new Claim("Permissions", "read,write") // Permissões personalizadas
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GerarRefreshToken()
        {
            var randomBytes = new byte[32];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }

            return Convert.ToBase64String(randomBytes);
        }



    }
}
