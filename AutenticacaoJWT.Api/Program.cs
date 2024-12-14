using AutenticacaoJWT.Api.Configuracao;
using AutenticacaoJWT.Aplicacao.Servico;
using AutenticacaoJWT.Aplicacao.ServicoInterface;
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


            var chave = new SwaggerConfigracao().CodigoChave();

            // Configuração de autenticação JWT
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
