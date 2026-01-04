using AutenticacaoJWT.Aplicacao.DTO;
using System.Security.Claims;

namespace AutenticacaoJWT.Api.Configuracao
{
    public class ValidacaoMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidacaoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var url = context.Request.Path;
            Console.WriteLine($" Requisição recebida para: {url}");

            try
            {
                // Pula validação em rotas públicas
                if (url.StartsWithSegments("/api/Token/GerarToken"))
                {
                    await _next(context);
                    return;
                }

                string? tokenHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                string token = null;

                if (!string.IsNullOrEmpty(tokenHeader) && tokenHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = tokenHeader.Substring("Bearer ".Length).Trim();
                }

                if (!string.IsNullOrEmpty(token))
                {
                    TokenValidator validator = new TokenValidator();
                    UsuarioSessaoDto usuarioSessaoDto = validator.ValidarToken(token);


                    var claims = new List<Claim>
                    {
                        new Claim("Id", usuarioSessaoDto.IdUsuario.ToString()),
                        new Claim("Email", usuarioSessaoDto.Email),
                        new Claim("Nome", usuarioSessaoDto.Nome),
                        new Claim("Codigo", usuarioSessaoDto.Codigo ?? ""),
                        new Claim("Aplicativo", usuarioSessaoDto.Aplicativo ?? ""),
                        new Claim("DataCadastro", usuarioSessaoDto.DataCadastro.ToString("o")),
                        new Claim("UltimoAcesso", usuarioSessaoDto.UltimoAcesso?.ToString("o") ?? ""),
                        new Claim("Permissoes", string.Join(",", usuarioSessaoDto.Permissoes))
                    };

                    // Cria uma identidade baseada nas claims
                    var identity = new ClaimsIdentity(claims, "jwt"); 
                    // Cria um principal (usuário autenticado)
                    var principal = new ClaimsPrincipal(identity); 
                    // Armazena no HttpContext.User
                    context.User = principal;
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                await TratarErroAsync(context, ex);
            }

        }


        private async Task TratarErroAsync(HttpContext context, Exception exception)
        {
            var httpDicionarioCodigoErros = new Dictionary<Type, int>
            {
                { typeof(ArgumentException), StatusCodes.Status400BadRequest },
                { typeof(KeyNotFoundException), StatusCodes.Status404NotFound },
                { typeof(InvalidOperationException), StatusCodes.Status409Conflict },
                { typeof(UnauthorizedAccessException), StatusCodes.Status401Unauthorized },
                { typeof(FormatException), StatusCodes.Status422UnprocessableEntity },
                { typeof(NullReferenceException), StatusCodes.Status500InternalServerError }
            };

            var httpCodigoErro = httpDicionarioCodigoErros.TryGetValue(exception.GetType(), out var code)
                ? code
                : StatusCodes.Status500InternalServerError;

            var response = new
            {
                Codigo = httpCodigoErro,
                Mensagem = exception.Message
            };

            context.Response.StatusCode = httpCodigoErro;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(response);
        }

    }
}
  
