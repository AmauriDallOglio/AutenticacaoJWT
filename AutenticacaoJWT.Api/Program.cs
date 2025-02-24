using AutenticacaoJWT.Api.Configuracao;
using AutenticacaoJWT.Aplicacao.IServico;
using AutenticacaoJWT.Aplicacao.Servico;
using AutenticacaoJWT.Dominio.Entidade;
using AutenticacaoJWT.Dominio.InterfaceRepositorio;
using AutenticacaoJWT.Infra.Repositorio;

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
            builder.Services.InformacaoCabecalhoApi();
            builder.Services.VersionamentoApi();
            builder.Services.BotaoAutorizacaoToken();
            builder.Services.ConfiguracaoServicesJWT();
 


            builder.Services.AddControllers();

            builder.Services.AddTransient<IUsuarioRepositorio, UsuarioRepositorio>();
            builder.Services.AddScoped<ITokenServico, TokenServico>();
            builder.Services.AddScoped<ITokenConfiguracaoServico, TokenConfiguracaoServico>();

            builder.Services.AddScoped<Usuario>();
    

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.UseMiddleware<MiddlewareError>();

            app.Run();
        }
    }
}
