using AutenticacaoJWT.Api.Configuracao;
using AutenticacaoJWT.Aplicacao.Configuracao;


namespace AutenticacaoJWT.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {


            var builder = WebApplication.CreateBuilder(args);

            ConfiguracaoApi.BancoDeDados(builder.Services, builder.Configuration);
            ConfiguracaoApi.Controllers(builder.Services);
            ConfiguracaoApi.AutencicacaoJwt(builder.Services);
            ConfiguracaoApi.Autorizacao(builder.Services);
            ConfiguracaoApi.SwaggerVersionamento(builder.Services);
            ConfiguracaoApi.Cors(builder.Services);

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

            //app.UseMiddleware<ValidacaoMiddleware>();
            app.UseMiddleware<AutorizacaoMiddleware>();

            app.MapControllers();
            app.Run();
        }
    }
}
