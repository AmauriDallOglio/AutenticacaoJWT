using AutenticacaoJWT.Aplicacao.Controller.Token.GerarToken;
using AutenticacaoJWT.Aplicacao.Controller.Token.RefreshToken;
using AutenticacaoJWT.Aplicacao.Servico;
using AutenticacaoJWT.Aplicacao.Servico.Interface;
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
            services.AddScoped<GerartokenHandler>();

            services.AddScoped<RefreshTokenHandler>();

            services.AddScoped<ITokenConfiguracaoServico, TokenConfiguracaoServico>();
            services.AddScoped(typeof(IGenericoRepositorio<>), typeof(GenericoRepositorio<>));
            services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

            services.AddScoped<Usuario>();
        }
    }
}
