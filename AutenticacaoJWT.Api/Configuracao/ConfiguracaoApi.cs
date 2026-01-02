using AutenticacaoJWT.Infra.Contexto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;


namespace AutenticacaoJWT.Api.Configuracao
{
    public static class ConfiguracaoApi
    {

        public static void BancoDeDados(this IServiceCollection services, IConfigurationRoot configuration)
        {
       
            services.AddSqlServer<GenericoContexto>(configuration.GetConnectionString("ConexaoPadrao"));
        }

        public static void Controllers(this IServiceCollection services)
        {

            services.AddControllers();
        }


        public static void AutencicacaoJwt(this IServiceCollection services)
        {

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                   .AddJwtBearer(options =>
                   {
                       options.TokenValidationParameters = new TokenValidationParameters
                       {
                           ValidateIssuer = false,
                           ValidateAudience = false,
                           ValidateLifetime = true,
                           ValidateIssuerSigningKey = true,
                           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("minha_chave_secreta_super_segura"))
                       };
                   });
        }


        public static void SwaggerVersionamento(this IServiceCollection services)
        {

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.ConfigurarSwagger();
            });

            services.AddHttpContextAccessor();
        }

        public static void Cors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });
        }



        public static void Autorizacao(this IServiceCollection services)
        {

            services.AddAuthorization();
        }


         




        public static void ConfigurarSwagger(this SwaggerGenOptions c)
        {
            c.EnableAnnotations();

            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API do Serviço",
                Version = "v1",
                Description = "API para geração de token"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Informe: Bearer {seu token}"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
        }
    }
}

