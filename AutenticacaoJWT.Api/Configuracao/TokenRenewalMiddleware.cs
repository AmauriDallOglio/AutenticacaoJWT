using AutenticacaoJWT.Aplicacao.DTO;
using System.Text.Json;

namespace AutenticacaoJWT.Api.Configuracao
{
    public class TokenRenewalMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenRenewalMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Permite leitura do corpo da requisição várias vezes
            context.Request.EnableBuffering();

            if (context.Request.Method == HttpMethods.Post &&
                context.Request.Path.Equals("/api/Token/RefreshToken", StringComparison.OrdinalIgnoreCase))
            {
                // Lê o corpo da requisição
                var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();

                // Reposiciona o ponteiro para que outros middlewares/controladores possam ler o corpo
                context.Request.Body.Position = 0;

                // Desserializa o corpo para o DTO
                var refreshTokenDto = JsonSerializer.Deserialize<RefreshTokenDTO>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (refreshTokenDto != null && !string.IsNullOrEmpty(refreshTokenDto.Token))
                {
                    // Gera um novo acesso token


                }
            }

            await _next(context);
        }


        
    }
}
