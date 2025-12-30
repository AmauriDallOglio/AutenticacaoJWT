using AutenticacaoJWT.Api.Configuracao;
using AutenticacaoJWT.Aplicacao.Configuracao;
using AutenticacaoJWT.Infra.Contexto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace AutenticacaoJWT.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {


            var builder = WebApplication.CreateBuilder(args);

            // =======================================
            // BANCO DE DADOS
            // =======================================
            builder.Services.AddSqlServer<GenericoContexto>(
                builder.Configuration.GetConnectionString("ConexaoPadrao"));

            // =======================================
            // CONTROLLERS
            // =======================================
            builder.Services.AddControllers();

            // =======================================
            // AUTENTICAÇÃO JWT
            // =======================================
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

            // =======================================
            // AUTORIZAÇÃO
            // =======================================
            builder.Services.AddAuthorization();
            //builder.Services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("PermissaoLeitura", policy =>
            //        policy.RequireAssertion(context =>
            //            context.User.HasClaim(c =>
            //                c.Type == "Permissoes" &&
            //                c.Value.Split(',').Contains("read"))));

            //    options.AddPolicy("PermissaoEscrita", policy =>
            //        policy.RequireAssertion(context =>
            //            context.User.HasClaim(c =>
            //                c.Type == "Permissoes" &&
            //                c.Value.Split(',').Contains("write"))));
            //});

            // =======================================
            // SWAGGER + VERSIONAMENTO
            // =======================================

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.ConfigurarSwagger();
            });

            // =======================================
            // CORS
            // =======================================
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });

            // =======================================
            // DEPENDÊNCIAS
            // =======================================
            builder.Services.AddHttpContextAccessor();
            InjecaoDependencia.RegistrarServicosInjecaoDependencia(builder.Services);

   


            var app = builder.Build();

            // =======================================
            // PIPELINE
            // =======================================
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<AutorizacaoMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}
