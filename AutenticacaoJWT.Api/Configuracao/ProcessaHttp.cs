namespace AutenticacaoJWT.Api.Configuracao
{
    public class ProcessaHttp
    {
        private readonly RequestDelegate _next;
 

        public ProcessaHttp(RequestDelegate next )
        {
            _next = next;
 
        }

    }
}
