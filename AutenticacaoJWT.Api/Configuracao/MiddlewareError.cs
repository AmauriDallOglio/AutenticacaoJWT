using AutenticacaoJWT.Aplicacao.Controller.Token.GerarToken;
using AutenticacaoJWT.Aplicacao.DTO;
using AutenticacaoJWT.Aplicacao.Util;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace AutenticacaoJWT.Api.Configuracao
{
    public class MiddlewareError
    {

        private readonly RequestDelegate _next;
        private readonly string _caminhoLog = "logs/error_log.txt";

        private static PathString _url { get; set; } = PathString.Empty;
        private static string _metodo { get; set; } = string.Empty;
        private static string _token { get; set; } = string.Empty;


 

        public MiddlewareError(RequestDelegate next)
        {
            _next = next;
 
            if (!Directory.Exists("logs"))
            {
                Directory.CreateDirectory("logs");
            }
            _url = string.Empty;
            _metodo = string.Empty;
            _token = string.Empty;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            HelperConsoleColor.Info($"MiddlewareError 1 - Requisição recebida para: {context.Request.Path}");
            context.Request.EnableBuffering();
            _url = context.Request.Path;
            _metodo = context.Request.Method.ToString();
            _token = context.Request.Headers.Authorization.ToString();

            // Remove o prefixo "Bearer " (com ou sem espaço)
            if (!String.IsNullOrEmpty(_token))
            {
                _token = _token.Substring("Bearer ".Length).Trim();
            }

            if (context.Request.Method == HttpMethods.Post)
            {
                if (_url == "/api/Token/GerarToken")
                {
                    if (!string.IsNullOrEmpty(_token))
                    {
                        try
                        {
                            var key = Encoding.UTF8.GetBytes("minha_chave_secreta_super_segura");
                            var tokenHandler = new JwtSecurityTokenHandler();
                            var validationParameters = new TokenValidationParameters
                            {
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = new SymmetricSecurityKey(key),
                                ValidateIssuer = false,
                                ValidateAudience = false,
                                ValidateLifetime = true,
                                ClockSkew = TimeSpan.Zero
                            };

                            // Valida e descriptografa o token
                            ClaimsPrincipal? principal = tokenHandler.ValidateToken(_token, validationParameters, out SecurityToken validatedToken);

                            // Extrai as claims e converte para o modelo
                            UsuarioTokenModel usuarioToken = new UsuarioTokenModel
                            {
                                Id = Guid.Parse(GetClaimValue(principal, "Id")),
                                Email = GetClaimValue(principal, "Email"),
                                Nome = GetClaimValue(principal, "Nome"),
                                Codigo = GetClaimValue(principal, "Codigo"),
                                Aplicativo = GetClaimValue(principal, "Aplicativo"),
                                Refresh = GetClaimValue(principal, "Refresh"),
                                Permissions = GetClaimValue(principal, "Permissions"),
                                DataCadastro = DateTime.Parse(GetClaimValue(principal, "DataCadastro")),
                                UltimoAcesso = ParseNullableDateTime(GetClaimValue(principal, "UltimoAcesso"))
                            };
                        }
                        catch (SecurityTokenExpiredException)
                        {
                            throw new UnauthorizedAccessException("Token expirado.");
                        }
                        catch (SecurityTokenException)
                        {
                            throw new UnauthorizedAccessException("Token inválido.");
                        }
                        catch (Exception ex)
                        {
                            throw new UnauthorizedAccessException($"Erro ao validar token: {ex.Message}");
                        }
                    }
                }

                if (_url == "/api/Token/Refresh")
                {
                    Console.WriteLine("Refresh");
                    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
                    Console.WriteLine(requestBody);
                    context.Request.Body.Position = 0;
                    var refreshTokenDto = JsonSerializer.Deserialize<GerarTokenRequest>(requestBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    Console.WriteLine(refreshTokenDto);
                }

                if (_url == "/api/VersaoUm/GerarToken")
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(_token))
                        {
                            var key = Encoding.UTF8.GetBytes("minha_chave_secreta_super_segura");
                            var tokenHandler = new JwtSecurityTokenHandler();

                            var validationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = false,
                                ValidateAudience = false,
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = new SymmetricSecurityKey(key),
                                ClockSkew = TimeSpan.Zero
                            };

                            // Valida e descriptografa o token
                            ClaimsPrincipal principal = tokenHandler.ValidateToken(_token, validationParameters, out SecurityToken validatedToken);

                            // Extrai claims
                            var nome = principal.Identity?.Name ?? "";
                            var roles = principal.Claims
                                .Where(c => c.Type == ClaimTypes.Role)
                                .Select(c => c.Value)
                                .ToArray();

                            var versoes = principal.Claims
                                .Where(c => c.Type == "AcessoApi")
                                .Select(c => c.Value)
                                .ToArray();

                            var usuario = new UsuarioDto
                            {
                                Nome = nome,
                                Roles = roles,
                                Versoes = versoes
                            };

                            // Armazena o usuário decodificado no contexto (para uso posterior)
                            context.Items["UsuarioLogado"] = usuario;
                        }
                    }
                    catch (SecurityTokenExpiredException)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsJsonAsync(new { Codigo = 401, Mensagem = "Token expirado." });
                        return;
                    }
                    catch (Exception)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsJsonAsync(new { Codigo = 401, Mensagem = "Token inválido." });
                        return;
                    }
                    //Console.WriteLine("token");


                    //if (!string.IsNullOrEmpty(_token))
                    //{
                    //    if (!_token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    //    {
                    //        throw new UnauthorizedAccessException("ResourceMensagem.BearerNaoInformado");
                    //    }



                    //    if (string.IsNullOrEmpty(_token))
                    //        throw new UnauthorizedAccessException("Token é obrigatório.");

                    //    var key = Encoding.UTF8.GetBytes("minha_chave_secreta_super_segura");

                    //    var tokenHandler = new JwtSecurityTokenHandler();
                    //    try
                    //    {
                    //        var validationParameters = new TokenValidationParameters
                    //        {
                    //            ValidateIssuer = false,
                    //            ValidateAudience = false,
                    //            ValidateLifetime = true,
                    //            ValidateIssuerSigningKey = true,
                    //            IssuerSigningKey = new SymmetricSecurityKey(key),
                    //            ClockSkew = TimeSpan.Zero // evita tolerância extra de tempo
                    //        };

                    //        try
                    //        {
                    //            var principal = tokenHandler.ValidateToken(_token, validationParameters, out SecurityToken validatedToken);

                    //            // Extrai as claims do token
                    //            var nome = principal.Identity?.Name;
                    //            var roles = principal.Claims
                    //                .Where(c => c.Type == ClaimTypes.Role)
                    //                .Select(c => c.Value)
                    //                .ToArray();

                    //            var versoes = principal.Claims
                    //                .Where(c => c.Type == "AcessoApi")
                    //                .Select(c => c.Value)
                    //                .ToArray();

                    //            var resultdo = new UsuarioDto
                    //            {
                    //                Nome = nome ?? "",
                    //                Roles = roles,
                    //                Versoes = versoes
                    //            };
                    //        }
                    //        catch
                    //        {
                    //            throw new UnauthorizedAccessException(" null"); // Retorna null se o token for inválido ou expirado
                    //        }
                    //    }
                    //    catch (SecurityTokenExpiredException)
                    //    {
                    //        throw new UnauthorizedAccessException("Token expirado.");
                    //    }
                    //    catch (SecurityTokenException ex)
                    //    {
                    //        throw new UnauthorizedAccessException("Token inválido.");
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        throw new UnauthorizedAccessException("Erro inesperado.");
                    //    }
                    //}
                }
            }
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await TratamentoExceptionAsync(context, ex);
 
            }
            HelperConsoleColor.Info($"MiddlewareError 2 - Resposta enviada para: {context.Response.StatusCode + " / " + _url}");
        }



        public ClaimsPrincipal? ValidarToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes("minha_chave_secreta_super_segura");

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                return tokenHandler.ValidateToken(token, validationParameters, out _);
            }
            catch
            {
                return null;
            }
        }

        private string GetClaimValue(ClaimsPrincipal principal, string claimType)
        {
            return principal.FindFirst(claimType)?.Value ?? string.Empty;
        }

        private DateTime? ParseNullableDateTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return DateTime.TryParse(value, out var result) ? result : null;
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

            string mensagemDoLog = await new ArquivoLog().IncluirLinha(_caminhoLog, exception, _url, "Erro inesperado");


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
                    MensagemErroInserirCommandRequest requestErro = new MensagemErroInserirCommandRequest() { Descricao = mensagemDoLog, Chamada = _url };
                    MensagemErroInserirCommandResponse responseErro = mediator.Send(requestErro, new CancellationToken()).Result;
                }
            }
            catch (Exception mediatorEx)
            {
                await new ArquivoLog().IncluirLinha(_caminhoLog, mediatorEx, _url, "Erro no CreateScope");
            }
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(response);
        }


        private static async Task HandleExceptionAsync(HttpContext context, Exception ex, long tempoMs)
        {
            var versao = DetectarVersao(context.Request.Path);
            var response = CriarRespostaErro(versao, context.Response.StatusCode == 0 ? 500 : context.Response.StatusCode, ex.Message, tempoMs);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }


        private static string DetectarVersao(PathString path)
        {
            if (path.HasValue)
            {
                if (path.Value.Contains("/v1/", StringComparison.OrdinalIgnoreCase)) return "v1";
                if (path.Value.Contains("/v2/", StringComparison.OrdinalIgnoreCase)) return "v2";
                if (path.Value.Contains("/v3/", StringComparison.OrdinalIgnoreCase)) return "v3";
            }
            return "v1"; // padrão
        }

        private static object CriarRespostaErro(string versao, int statusCode, string mensagem, long tempoMs)
        {
            return versao switch
            {
                "v2" => new
                {
                    sucesso = false,
                    mensagem,
                    codigo = statusCode,
                    tempo = $"{tempoMs}ms"
                },
                "v3" => new
                {
                    sucesso = false,
                    mensagem,
                    codigo = statusCode,
                    tempo = $"{tempoMs}ms",
                    detalhes = "Verifique os parâmetros enviados ou contate o suporte técnico."
                },
                _ => new
                {
                    sucesso = false,
                    mensagem
                }
            };
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
