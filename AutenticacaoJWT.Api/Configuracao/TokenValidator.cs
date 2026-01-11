using AutenticacaoJWT.Aplicacao.DTO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AutenticacaoJWT.Api.Configuracao
{
    public class TokenValidator
    {
        private readonly string _secretKey = "minha_chave_secreta_super_segura";

        public UsuarioSessaoDto ValidarToken(string token)
        {

            // Log útil para depuração
     
            Console.WriteLine($"Token extraído: '{token}'");
            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException("Token não informado.");

 
            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException("Token não informado.");

            if (token.Split('.').Length != 3)
                throw new FormatException("Token malformado: não possui 3 partes.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                // Valida e descriptografa o token
                ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                // Extrai as claims e converte para o modelo
                UsuarioSessaoDto usuarioToken = new UsuarioSessaoDto
                {
                    IdUsuario = Guid.Parse(ObterClaim(claimsPrincipal, "Id")),
                    Email = ObterClaim(claimsPrincipal, "Email"),
                    Nome = ObterClaim(claimsPrincipal, "Nome"),
                    Codigo = ObterClaim(claimsPrincipal, "Codigo"),
                    Aplicativo = ObterClaim(claimsPrincipal, "Aplicativo"),
                    Permissoes = claimsPrincipal.FindFirst("Permissoes")?.Value.Split(',') ?? [],
                    DataCadastro = DateTime.Parse(ObterClaim(claimsPrincipal, "DataCadastro")),
                    UltimoAcesso = CoverterStringEmDatetime(ObterClaim(claimsPrincipal, "UltimoAcesso"))
                };

                if (string.IsNullOrEmpty(usuarioToken.Email)) 
                    throw new UnauthorizedAccessException("Claim 'Email' não encontrada no token.");

                return usuarioToken;

            }
            catch (SecurityTokenExpiredException)
            {
                throw new UnauthorizedAccessException("Token expirado.");
            }
            catch (SecurityTokenException)
            {
                throw new UnauthorizedAccessException("Token inválido.");
            }
        }
        private string ObterClaim(ClaimsPrincipal principal, string claimType)
        {
            return principal.FindFirst(claimType)?.Value ?? string.Empty;
        }

        private DateTime? CoverterStringEmDatetime(string value)
        {
            // Se a string for nula, vazia ou só espaços em branco, retorna null
            if (string.IsNullOrWhiteSpace(value))
                return null;

            // Tenta converter a string para DateTime
            // Se conseguir, retorna o valor convertido
            // Se não conseguir, retorna null
            return DateTime.TryParse(value, out var result) ? result : null;
        }
    }

}
