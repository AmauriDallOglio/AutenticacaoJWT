using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;
using System.Text.Json;


namespace AutenticacaoJWT.Api.Configuracao
{
    internal static class ConfiguracaoApi
    {

        // ============================
        // JWT
        // ============================
        public static void ConfiguracaoAutenticacaoJWT(this IServiceCollection services, IConfiguration configuration)
        {
            //var secret = "minha_chave_secreta_super_segura";  //configuration["Jwt:Secret"];
            //if (string.IsNullOrEmpty(secret) || secret.Length < 32)
            //    throw new ArgumentException("A chave secreta deve ter pelo menos 32 caracteres.");

            //var chave = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));


            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(options =>
            //{
            //    options.RequireHttpsMetadata = false;
            //    options.SaveToken = true;
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = chave,
            //        ValidateIssuer = false,
            //        ValidateAudience = false,
            //        ValidateLifetime = true,
            //        RoleClaimType = ClaimTypes.Role,
            //        ClockSkew = TimeSpan.Zero
            //    };

            //    options.Events = new JwtBearerEvents
            //    {
            //        OnChallenge = context =>
            //        {
            //            context.HandleResponse();
            //            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //            context.Response.ContentType = "application/json";
            //            var result = JsonSerializer.Serialize(new
            //            {
            //                sucesso = false,
            //                code = 401,
            //                message = "Token inválido ou expirado. Faça login novamente."
            //            });
            //            return context.Response.WriteAsync(result);
            //        },
            //        OnForbidden = context =>
            //        {
            //            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            //            context.Response.ContentType = "application/json";
            //            var result = JsonSerializer.Serialize(new
            //            {
            //                sucesso = false,
            //                code = 403,
            //                message = "Acesso negado. Você não possui permissão para acessar este recurso."
            //            });
            //            return context.Response.WriteAsync(result);
            //        }
            //    };
            //});

            // ============================================================
            //  AUTENTICAÇÃO JWT
            // ============================================================
            var key = "minha_chave_secreta_super_segura";
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                               message = "Token inválido ou expirado. Faça login novamente."
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
                               message = "Acesso negado. Você não possui permissão para acessar este recurso."
                           });
                           return context.Response.WriteAsync(result);
                       }
                   };
               });

        }

        // ============================
        // Versionamento de API
        // ============================
        public static void ConfiguracaoVersionamentoApi(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });


        }

        // ============================
        // Swagger com múltiplas versões + JWT
        // ============================
        public static void ConfiguracaoSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();

                // Documentação por versão
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "API v1",
                    Description = "Documentação da versão 1 da API"
                });
                options.SwaggerDoc("v2", new OpenApiInfo
                {
                    Version = "v2",
                    Title = "API v2",
                    Description = "Documentação da versão 2 da API"
                });
                options.SwaggerDoc("v3", new OpenApiInfo { Title = "API v3", Version = "v3.0", Description = $"Documentação da API versão 3" });

                // Autenticação JWT
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Insira o token JWT no cabeçalho com o prefixo 'Bearer '",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
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

        }

        // ============================
        // Políticas de Autorização por versão
        // ============================
        public static void ConfiguracaoPoliticasAutorizacao(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AcessoV1", policy => policy.RequireClaim("AcessoApi", "v1"));
                options.AddPolicy("AcessoV2", policy => policy.RequireClaim("AcessoApi", "v2"));
                options.AddPolicy("AcessoV3", policy => policy.RequireClaim("AcessoApi", "v3"));
            });
        }

        public static void ConfiguracaoSwaggerUI(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }

                options.DocExpansion(DocExpansion.List);
                options.RoutePrefix = "swagger";
                options.DisplayRequestDuration();
            });
        }

        
    }
}

