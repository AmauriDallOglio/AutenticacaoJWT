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

            await _next(context);
        }
    }
}
  
