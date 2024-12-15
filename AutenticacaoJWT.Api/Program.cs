using AutenticacaoJWT.Api.Configuracao;
using AutenticacaoJWT.Aplicacao.Servico;
using AutenticacaoJWT.Aplicacao.ServicoInterface;
using AutenticacaoJWT.Dominio.Entidade;
using AutenticacaoJWT.Dominio.InterfaceRepositorio;
using AutenticacaoJWT.Infra.Repositorio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace AutenticacaoJWT.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var chave = new TokenConfigracao().CodigoChave();

            // Configura��o de autentica��o JWT
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "JwtInMemoryAuth",
                        ValidAudience = "JwtInMemoryAuth",
                        IssuerSigningKey = new SymmetricSecurityKey(chave)
                    };
                });

            builder.Services.AddScoped<IUsuarioServico, UsuarioServico>();
            builder.Services.AddControllers();

            builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
            builder.Services.AddTransient<Usuario>();

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.UseMiddleware<TokenRenewalMiddleware>();

            app.Run();
        }
    }
}
