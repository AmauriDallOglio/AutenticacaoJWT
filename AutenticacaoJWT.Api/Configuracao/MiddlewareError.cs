using AutenticacaoJWT.Aplicacao.Request;
using AutenticacaoJWT.Aplicacao.Util;
using MediatR;
using System.Text.Json;

namespace AutenticacaoJWT.Api.Configuracao
{
    public class MiddlewareError
    {

        private readonly RequestDelegate _next;
        private readonly string _caminhoLog = "logs/error_log.txt";

        private static PathString _PathString { get; set; }

        public MiddlewareError(RequestDelegate next)
        {
            _next = next;
 
            if (!Directory.Exists("logs"))
            {
                Directory.CreateDirectory("logs");
            }
            _PathString = string.Empty;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            HelperConsoleColor.Info($"MiddlewareError 1 - Requisição recebida para: {context.Request.Path}");
            context.Request.EnableBuffering();

            if (context.Request.Method == HttpMethods.Post)
            {
                if (context.Request.Path.Equals("/api/Token/Login", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("token");
                    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
                    Console.WriteLine(requestBody);
                    context.Request.Body.Position = 0;
                    var refreshTokenDto = JsonSerializer.Deserialize<TokenRequest>(requestBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    Console.WriteLine(refreshTokenDto);
                }

                if (context.Request.Path.Equals("/api/Token/Refresh", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Refresh");
                    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
                    Console.WriteLine(requestBody);
                    context.Request.Body.Position = 0;
                    var refreshTokenDto = JsonSerializer.Deserialize<TokenRequest>(requestBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    Console.WriteLine(refreshTokenDto);
                }
            }

            _PathString = context.Request.Path;
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await TratamentoExceptionAsync(context, ex);
            }
            HelperConsoleColor.Info($"MiddlewareError 2 - Resposta enviada para: {context.Response.StatusCode + " / " + _PathString}");
        }

        private async Task TratamentoExceptionAsync(HttpContext context, Exception exception)
        {
            var httpDicionarioCodigoErros = new Dictionary<Type, int>
            {
                { typeof(ArgumentException), StatusCodes.Status400BadRequest },
                { typeof(KeyNotFoundException), StatusCodes.Status404NotFound },
                { typeof(InvalidOperationException), StatusCodes.Status409Conflict },
                { typeof(UnauthorizedAccessException), StatusCodes.Status401Unauthorized },
                { typeof(FormatException), StatusCodes.Status422UnprocessableEntity },
                { typeof(NullReferenceException), StatusCodes.Status500InternalServerError },
                { typeof(DivideByZeroException), StatusCodes.Status400BadRequest },
                { typeof(MinhaExcecaoPersonalizada), StatusCodes.Status418ImATeapot }
            };

            var httpCodigoErro = httpDicionarioCodigoErros.TryGetValue(exception.GetType(), out var code) ? code : StatusCodes.Status500InternalServerError;

            string mensagemDoLog = await new ArquivoLog().IncluirLinha(_caminhoLog, exception, _PathString, "Erro inesperado");


            var response = new
            {
                Codigo = httpCodigoErro,
                Mensagem = exception.Message,
                Detalhe = mensagemDoLog,
            };

            try
            {
                using (var scope = context.RequestServices.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    MensagemErroInserirCommandRequest requestErro = new MensagemErroInserirCommandRequest() { Descricao = mensagemDoLog, Chamada = _PathString };
                    MensagemErroInserirCommandResponse responseErro = mediator.Send(requestErro, new CancellationToken()).Result;
                }
            }
            catch (Exception mediatorEx)
            {
                await new ArquivoLog().IncluirLinha(_caminhoLog, mediatorEx, _PathString, "Erro no CreateScope");
            }
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(response);
        }


        //private readonly RequestDelegate _next;

        //public MiddlewareError(RequestDelegate next)
        //{
        //    _next = next;
        //}

        //public async Task InvokeAsync(HttpContext context)
        //{
        //    try
        //    {
        //        await _next(context);
        //    }
        //    catch (Exception ex)
        //    {
        //        await HandleExceptionAsync(context, ex);
        //    }
        //}

        //private Task HandleExceptionAsync(HttpContext context, Exception exception)
        //{
        //    HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
        //    string error = "An unexpected error occurred.";
        //    string errorDescription = exception.Message;

        //    if (exception is InvalidGrantException)
        //    {
        //        statusCode = HttpStatusCode.BadRequest;
        //        error = "invalid_grant";
        //        errorDescription = "Os motivos são vários, pode ser porque o authorization_code ou refresh_token são inválidos, expiraram ou foram revogados, foram enviados em um fluxo incorreto, pertencem a outro cliente ou o redirect_uri usado no fluxo de autorização não corresponde ao que tem configurado seu aplicativo."; //"Error validating grant. Your authorization code or refresh token may be expired or it was already used.";
        //    }
        //    else if (exception is InvalidClientException)
        //    {
        //        statusCode = HttpStatusCode.BadRequest;
        //        error = "invalid_client";
        //        // errorDescription = "The client_id and/or client_secret provided are invalid.";
        //        errorDescription = "O client_id e/ou client_secret do seu aplicativo fornecido é inválido.";
        //    }
        //    else if (exception is InvalidScopeException)
        //    {
        //        statusCode = HttpStatusCode.BadRequest;
        //        error = "invalid_scope";
        //        errorDescription = "O alcance solicitado é inválido, desconhecido ou foi criado no formato errado. Os valores permitidos para o parâmetro alcance são: ‘offline_access’, ‘write’ e ‘read’."; // "The requested scope is invalid, unknown, or malformed.";
        //    }
        //    else if (exception is InvalidRequestException)
        //    {
        //        statusCode = HttpStatusCode.BadRequest;
        //        error = "invalid_request";
        //        errorDescription = "A solicitação não inclui um parâmetro obrigatório, inclui um parâmetro ou valor de parâmetro não suportado, tem algum valor dobrado ou está mal formado."; //"The request is missing a required parameter, includes an unsupported parameter or value, has a duplicated parameter, or is malformed.";
        //    }
        //    else if (exception is UnsupportedGrantTypeException)
        //    {
        //        statusCode = HttpStatusCode.BadRequest;
        //        error = "unsupported_grant_type";
        //        errorDescription = "Os valores permitidos para grant_type são ‘authorization_code’ ou ‘refresh_token’."; // "The provided grant_type is not supported.";
        //    }
        //    else if (exception is ForbiddenException)
        //    {
        //        statusCode = HttpStatusCode.Forbidden;
        //        error = "forbidden";
        //        errorDescription = "A chamada não autoriza o acesso, possivelmente está sendo usado o token de outro usuário, ou para o caso de grant o usuário não tem acesso à URL de Mercado Livre de seu país (.ar, .br, .mx, etc) e deve verificar que sua conexão ou navegador funcione corretamente para os domínios do MELI.";// "The request is not authorized. Ensure you are using the correct user token or check access permissions.";
        //    }
        //    else if (exception is LocalRateLimitedException)
        //    {
        //        statusCode = HttpStatusCode.TooManyRequests;
        //        error = "local_rate_limited";
        //        errorDescription = "Por excessivas requisições, são bloqueadas temporariamente as chamadas. Volte a tentar em alguns segundos."; // "Too many requests. Please try again later.";
        //    }
        //    else if (exception is UnauthorizedClientException)
        //    {
        //        statusCode = HttpStatusCode.Unauthorized;
        //        error = "unauthorized_client";
        //        errorDescription = "A aplicação não tem grant com o usuário ou as permissões (scopes) que tem o aplicativo com esse usuário. Não permitem criar um token."; // "The application does not have the necessary grants with the user.";
        //    }
        //    else if (exception is UnauthorizedApplicationException)
        //    {
        //        statusCode = HttpStatusCode.Unauthorized;
        //        error = "unauthorized_application";
        //        errorDescription = "A aplicação está bloqueada, e por isso não poderá operar até resolver o problema."; // "The application is blocked and cannot operate until the issue is resolved.";
        //    }

        //    var response = new
        //    {
        //        error,
        //        error_description = errorDescription
        //    };

        //    context.Response.ContentType = "application/json";
        //    context.Response.StatusCode = (int)statusCode;

        //    return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        //}


        //public class InvalidGrantException : Exception
        //{
        //    public InvalidGrantException() { }

        //    public InvalidGrantException(string message) : base(message) { }

        //    public InvalidGrantException(string message, Exception inner) : base(message, inner) { }
        //}

        //public class InvalidClientException : Exception
        //{
        //    public InvalidClientException() { }

        //    public InvalidClientException(string message) : base(message) { }

        //    public InvalidClientException(string message, Exception inner) : base(message, inner) { }
        //}

        //public class InvalidScopeException : Exception
        //{
        //    public InvalidScopeException() { }

        //    public InvalidScopeException(string message) : base(message) { }

        //    public InvalidScopeException(string message, Exception inner) : base(message, inner) { }
        //}

        //public class InvalidRequestException : Exception
        //{
        //    public InvalidRequestException() { }

        //    public InvalidRequestException(string message) : base(message) { }

        //    public InvalidRequestException(string message, Exception inner) : base(message, inner) { }
        //}

        //public class UnsupportedGrantTypeException : Exception
        //{
        //    public UnsupportedGrantTypeException() { }

        //    public UnsupportedGrantTypeException(string message) : base(message) { }

        //    public UnsupportedGrantTypeException(string message, Exception inner) : base(message, inner) { }
        //}

        //public class ForbiddenException : Exception
        //{
        //    public ForbiddenException() { }

        //    public ForbiddenException(string message) : base(message) { }

        //    public ForbiddenException(string message, Exception inner) : base(message, inner) { }
        //}

        //public class LocalRateLimitedException : Exception
        //{
        //    public LocalRateLimitedException() { }

        //    public LocalRateLimitedException(string message) : base(message) { }

        //    public LocalRateLimitedException(string message, Exception inner) : base(message, inner) { }
        //}

        //public class UnauthorizedClientException : Exception
        //{
        //    public UnauthorizedClientException() { }

        //    public UnauthorizedClientException(string message) : base(message) { }

        //    public UnauthorizedClientException(string message, Exception inner) : base(message, inner) { }
        //}

        //public class UnauthorizedApplicationException : Exception
        //{
        //    public UnauthorizedApplicationException() { }

        //    public UnauthorizedApplicationException(string message) : base(message) { }

        //    public UnauthorizedApplicationException(string message, Exception inner) : base(message, inner) { }
        //}

    }
}
