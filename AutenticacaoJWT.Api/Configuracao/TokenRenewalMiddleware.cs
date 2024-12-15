using AutenticacaoJWT.Aplicacao.Request;
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
            context.Request.EnableBuffering();

            if (context.Request.Method == HttpMethods.Post && context.Request.Path.Equals("/api/Token/RefreshToken", StringComparison.OrdinalIgnoreCase))
            {
                var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
                context.Request.Body.Position = 0;
                var refreshTokenDto = JsonSerializer.Deserialize<TokenRequest>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (refreshTokenDto != null && !string.IsNullOrEmpty(refreshTokenDto.Token))
                {


                }
            }

            await _next(context);
        }


        
    }
}
