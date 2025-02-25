using AutenticacaoJWT.Api.Configuracao;
using AutenticacaoJWT.Aplicacao.Configuracao;
using AutenticacaoJWT.Infra.Contexto;
using Microsoft.EntityFrameworkCore;

namespace AutenticacaoJWT.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddSqlServer<GenericoContexto>(builder.Configuration.GetConnectionString("ConexaoPadrao"));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.InformacaoCabecalhoApi();
            builder.Services.VersionamentoApi();
            builder.Services.BotaoAutorizacaoToken();
            builder.Services.ConfiguracaoServicesJWT();

            InjecaoDependencia.RegistrarServicosInjecaoDependencia(builder.Services);


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
