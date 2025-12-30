using AutenticacaoJWT.Aplicacao.Controller.Token.GerarToken;
using AutenticacaoJWT.Aplicacao.DTO;
using AutenticacaoJWT.Aplicacao.Util;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace AutenticacaoJWT.Api.Configuracao
{
    public class AutorizacaoMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly string _caminhoLog = "logs/error_log.txt";

        private static PathString _url { get; set; } = PathString.Empty;
        private static string _metodo { get; set; } = string.Empty;
        private static string _token { get; set; } = string.Empty;

 
        private readonly ILogger<AutorizacaoMiddleware> _logger;


        public AutorizacaoMiddleware(RequestDelegate next, ILogger<AutorizacaoMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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
            if (!string.IsNullOrEmpty(_token))
            {
                _token = _token.Substring("Bearer ".Length).Trim();
            }

            try
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
                        UsuarioSessaoDto usuarioToken = new UsuarioSessaoDto
                        {
                            IdUsuario = Guid.Parse(GetClaimValue(principal, "Id")),
                            Email = GetClaimValue(principal, "Email"),
                            Nome = GetClaimValue(principal, "Nome"),
                            Codigo = GetClaimValue(principal, "Codigo"),
                            Aplicativo = GetClaimValue(principal, "Aplicativo"),
                            Permissoes = principal.FindFirst("Permissoes")?.Value.Split(',') ?? [],
                            DataCadastro = DateTime.Parse(GetClaimValue(principal, "DataCadastro")),
                            UltimoAcesso = ParseNullableDateTime(GetClaimValue(principal, "UltimoAcesso"))
                        };
                    }
                    catch (SecurityTokenExpiredException ex)
                    {
                         _logger.LogError(ex, $"Token expirado: {ex.Message}");

                        throw;

                    }
                    catch (SecurityTokenException ex)
                    {
                        _logger.LogError(ex, $"Token inválido: {ex.Message}");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Erro ao validar token: {ex.Message}");
                        throw;
                    }
                }


                if (context.Request.Method == HttpMethods.Get)
                {
                    if (_url == "/api/Token/publica")
                    {

                    }
                }

                if (context.Request.Method == HttpMethods.Post)
                {
                    if (_url == "/api/Token/GerarToken")
                    {

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
                    }
                }

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






    }
}
