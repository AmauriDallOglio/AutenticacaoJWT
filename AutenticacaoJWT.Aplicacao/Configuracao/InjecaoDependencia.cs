using AutenticacaoJWT.Aplicacao.IServico;
using AutenticacaoJWT.Aplicacao.Servico;
using AutenticacaoJWT.Dominio.Entidade;
using AutenticacaoJWT.Dominio.InterfaceRepositorio;
using AutenticacaoJWT.Infra.Repositorio;
using Microsoft.Extensions.DependencyInjection;

namespace AutenticacaoJWT.Aplicacao.Configuracao
{
    public static class InjecaoDependencia
    {
        public static void RegistrarServicosInjecaoDependencia(IServiceCollection services)
        {

 
            services.AddScoped<ITokenServico, TokenServico>();
            services.AddScoped<ITokenConfiguracaoServico, TokenConfiguracaoServico>();
            services.AddScoped(typeof(IGenericoRepositorio<>), typeof(GenericoRepositorio<>));
            services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

            services.AddScoped<Usuario>();
        }
    }
}
