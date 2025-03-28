using AutenticacaoJWT.Api.Configuracao;
using AutenticacaoJWT.Aplicacao.Configuracao;
using AutenticacaoJWT.Infra.Contexto;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AutenticacaoJWT.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            HelperConsoleColor.Alerta("Inicializando");

            builder.Services.AddSqlServer<GenericoContexto>(builder.Configuration.GetConnectionString("ConexaoPadrao"));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.InformacaoCabecalhoApi();
            builder.Services.VersionamentoApi();
            //builder.Services.BotaoAutorizacaoToken();
            builder.Services.ConfiguracaoServicesJWT();

            InjecaoDependencia.RegistrarServicosInjecaoDependencia(builder.Services);


            var app = builder.Build();
            HelperConsoleColor.Info("App - Configure the HTTP request pipeline.");
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.Use(async (context, next) =>
            {
                var stopwatch = Stopwatch.StartNew(); // Inicia a medição do tempo
                HelperConsoleColor.Alerta($"Program 1 - Middleware antes do controlador");
 
                await next.Invoke(); // Chama o próximo middleware

                stopwatch.Stop(); // Para a medição do tempo
                HelperConsoleColor.Alerta($"Program 2 - Middleware depois do controlador. Tempo decorrido: {stopwatch.ElapsedMilliseconds} ms");
            });


            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.UseMiddleware<MiddlewareError>();
            HelperConsoleColor.Sucesso("Finalizado!");
            app.Run();
        }
    }
}
