﻿using AutenticacaoJWT.Aplicacao.Servico;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Security.Claims;
using System.Text;

namespace AutenticacaoJWT.Api.Configuracao
{
    internal static class ConfiguracaoBearer
    {
        public static void VersionamentoApi(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true; //  a versão padrão (1.0 neste caso) será usada automaticamente.
                options.DefaultApiVersion = new ApiVersion(1, 0); // Se a versão não for especificada na solicitação, esta será a versão usada.
                options.ReportApiVersions = true; // Versões da API devem ser relatadas nas respostas.
            });
        }

        public static void InformacaoCabecalhoApi(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Gerenciador de autorização.",
                    Description = "O processo de autenticação é utilizado para verificar a identidade de uma pessoa em função de um ou vários fatores, garantindo que os dados de quem os enviou sejam corretos.",
                    TermsOfService = new Uri("https://google.com.br/"),
                    Contact = new OpenApiContact
                    {
                        Name = "Amauri",
                        Email = "amauri@hotmail.com",
                        Url = new Uri("https://google.com.br/")
                    }
                });
            });
        }

        public static void BotaoAutorizacaoToken(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Insira o token JWT no cabeçalho com o prefixo 'Bearer '",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });
        }


        public static string RetornaSecret(IConfiguration configuration)
        {
            var secret = configuration["Jwt:Secret"];

            if (secret.Length < 32)
            {
         
                throw new ArgumentException("A chave secreta não localizada.");
            }
            return secret;
        }

        public static byte[] RetornaSecretASCII(string secret)
        {
            var array = Encoding.ASCII.GetBytes(secret);
            if (array.Length < 32)
            {
                throw new ArgumentException("A chave secreta deve ter pelo menos 256 bits (32 caracteres) para o algoritmo HMAC-SHA256.");
            }
            return array;
        }

        public static void ConfiguracaoServicesJWT(this IServiceCollection services)
        {
            var codigoSecret = new TokenConfiguracaoServico().CodigoSecret();
            var chave = new TokenConfiguracaoServico().CodigoChave();


 
            byte[] secretASCII = RetornaSecretASCII(codigoSecret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, //Se definido como true, o sistema tentará validar a chave de assinatura
                    IssuerSigningKey = new SymmetricSecurityKey(chave), //Esta é a chave usada para verificar a assinatura dos tokens.
                    ValidateIssuer = false, //Se definido como false, o sistema não realizará essa validação quem emitiu o token.
                    ValidateAudience = false, //Se definido como false, o sistema não realizará essa validação quem o token é destinado.
                    RoleClaimType = ClaimTypes.Role, //Isso é útil quando você deseja mapear as funções dos usuários para reivindicações no token.
                    ClockSkew = TimeSpan.Zero, //Qualquer atraso ou adiantamento na hora do sistema pode resultar na rejeição do token.
                    ValidateLifetime = false, //Se definido como true, o swagger verificará se o token já expirou automaticamente.
                    ValidIssuer = "JwtInMemoryAuth",
                    ValidAudience = "JwtInMemoryAuth",
                };
            });


        }

        internal static void ConfiguracaoSwagger(this IApplicationBuilder app)
        {
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", typeof(Program).Assembly.GetName().Name);

                // Adiciona o botão "Authorize" ao Swagger UI
                options.DocExpansion(DocExpansion.List);
                options.RoutePrefix = "swagger";
                options.DisplayRequestDuration();
            });
        }
    }
}

