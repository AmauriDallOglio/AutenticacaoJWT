using AutenticacaoJWT.Aplicacao.Servico.Interface;
using AutenticacaoJWT.Dominio.Entidade;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AutenticacaoJWT.Aplicacao.Servico
{
    public sealed class TokenConfiguracaoServico : ITokenConfiguracaoServico
    {
        private readonly string _secretKey = "SuperSecretKey@1234567890123456abcdefghijlmn";
        private readonly string _issuer = "JwtInMemoryAuth";
        private readonly string _audience = "JwtInMemoryAuth";
        public byte[] CodigoChave()
        {
            byte[] chave = Encoding.UTF8.GetBytes(_secretKey);
            if (chave.Length < 32)
            {
                throw new Exception($"A chave precisa ter pelo menos 32 bytes. Chave atual: {chave.Length} bytes.");
            }
            return chave;
        }

        public string CodigoSecret()
        {
            return _secretKey;
        }

        private byte[] DerivarChavePersonalizada(string email, string refresh)
        {
            // Combina a chave mestre com informações únicas do usuário
            string baseString = $"{_secretKey}:{email}:{refresh}";
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(baseString));
        }


        public string GerarJwtToken(Usuario usuario)
        {

            //var userKey = DerivarChavePersonalizada(usuario.Email, usuario.Refresh);
            //var securityKey = new SymmetricSecurityKey(userKey);
            //var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            var claims = new[]
            {
                new Claim("Email", usuario.Email),
                //new Claim("AppId", "appId"),  // Identifica o aplicativo
                new Claim("Nome", usuario.Nome),  // Nome do usuário
            //    new Claim("Id", usuario.Id.ToString()),  // ID do usuário
                new Claim("Refresh", usuario.Refresh),  // Papel do usuário
                //new Claim("CustomClaim", "CustomValue"), // Claim personalizada
                //new Claim("DateCreated", DateTime.Now.ToString()),  // Data de criação
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

    
        //public string GerarRefreshToken()
        //{
        //    var randomBytes = new byte[32];
        //    using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
        //    {
        //        rng.GetBytes(randomBytes);
        //    }

        //    return Convert.ToBase64String(randomBytes);
        //}

        public string GerarRefresh()
        {
            byte[] randomBytes = new byte[32];
            RandomNumberGenerator.Fill(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }


    }
}
