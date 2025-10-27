using AutenticacaoJWT.Api.Configuracao;
using AutenticacaoJWT.Aplicacao.Configuracao;
using AutenticacaoJWT.Infra.Contexto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;


namespace AutenticacaoJWT.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            HelperConsoleColor.Alerta("Inicializando");

            builder.Services.AddSqlServer<GenericoContexto>(builder.Configuration.GetConnectionString("ConexaoPadrao"));



            ////if (!builder.Environment.IsDevelopment())
            ////{
            //string AZURE_DB = Environment.GetEnvironmentVariable("AZURE_DB");
            //Console.WriteLine("AZURE_DB: " + AZURE_DB);
            //builder.Services.AddDbContext<GenericoContexto>(options => options.UseSqlServer(AZURE_DB));
            ////}
            ////else
            ////{
            ////    string filePath = "C:\\Amauri\\GitHub\\ServicosNetAzureWebConnection.txt";
            ////    string AZURE_DB = File.ReadAllText(filePath).Replace("\\\\", "\\");
            ////    Console.WriteLine("AZURE_DB: " + AZURE_DB);
            ////    builder.Services.AddDbContext<GenericoContexto>(options => options.UseSqlServer(AZURE_DB));

            ////    //builder.Services.AddSqlServer<NetAzureContexto>(builder.Configuration.GetConnectionString("ConexaoPadrao"));
            ////}



            // ============================================================
            //  AUTENTICA��O JWT
            // ============================================================
            var key = "minha_chave_secreta_super_segura";

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                    };

                    // Respostas JSON padronizadas para erros 401 e 403
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            var result = JsonSerializer.Serialize(new
                            {
                                sucesso = false,
                                code = 401,
                                message = "Token inv�lido ou expirado. Fa�a login novamente."
                            });
                            return context.Response.WriteAsync(result);
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";
                            var result = JsonSerializer.Serialize(new
                            {
                                sucesso = false,
                                code = 403,
                                message = "Acesso negado. Voc� n�o possui permiss�o para acessar este recurso."
                            });
                            return context.Response.WriteAsync(result);
                        }
                    };
                });

            // ============================================================
            //  VERSIONAMENTO DE API
            // ============================================================
            builder.Services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader(); // usa /v1/, /v2/, etc.
            });

            builder.Services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV"; // gera nomes "v1", "v2", ...
                options.SubstituteApiVersionInUrl = true;
            });

            // ============================================================
            //  POL�TICAS DE AUTORIZA��O (por vers�o)
            // ============================================================
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AcessoV1", policy => policy.RequireClaim("AcessoApi", "v1"));
                options.AddPolicy("AcessoV2", policy => policy.RequireClaim("AcessoApi", "v2"));
                options.AddPolicy("AcessoV3", policy => policy.RequireClaim("AcessoApi", "v3"));
            });

            // ============================================================
            //  CONTROLLERS
            // ============================================================
            builder.Services.AddControllers();

            // ============================================================
            //  SWAGGER (com suporte a m�ltiplas vers�es e JWT)
            // ============================================================
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                // Documenta��o para m�ltiplas vers�es
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "API v1", Version = "v1.0", Description = $"Documenta��o da API vers�o 1" });
                options.SwaggerDoc("v2", new OpenApiInfo { Title = "API v2", Version = "v2.0", Description = $"Documenta��o da API vers�o 2" });
                options.SwaggerDoc("v3", new OpenApiInfo { Title = "API v3", Version = "v3.0", Description = $"Documenta��o da API vers�o 3" });

                // Suporte a autentica��o JWT no Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira o token JWT:"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });


            InjecaoDependencia.RegistrarServicosInjecaoDependencia(builder.Services);


            var app = builder.Build();

            HelperConsoleColor.Info("App - Configure the HTTP request pipeline.");

            // --- Swagger UI (gera endpoints din�micos por vers�o) ---
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant()
                    );
                }
            });

  

            app.Use(async (context, next) =>
            {
                var stopwatch = Stopwatch.StartNew(); // Inicia a medi��o do tempo
                HelperConsoleColor.Alerta($"app.Use 1 - Middleware antes do controlador");

                await next.Invoke(); // Chama o pr�ximo middleware

                stopwatch.Stop(); // Para a medi��o do tempo
                HelperConsoleColor.Alerta($"app.Use 2 - Middleware depois do controlador. Tempo decorrido: {stopwatch.ElapsedMilliseconds} ms");
            });


            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.UseMiddleware<MiddlewareError>();

            app.Run();
            HelperConsoleColor.Sucesso("Finalizado!");
        }
    }
}
