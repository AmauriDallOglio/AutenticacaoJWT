using AutenticacaoJWT.Aplicacao.IServico;
using AutenticacaoJWT.Aplicacao.Request;
using AutenticacaoJWT.Aplicacao.Response;
using AutenticacaoJWT.Dominio.Entidade;
using AutenticacaoJWT.Dominio.InterfaceRepositorio;

namespace AutenticacaoJWT.Aplicacao.Servico
{
    public class RefreshServico : IRefreshServico
    {

        public readonly IUsuarioRepositorio _IUsuarioRepositorio;
        public readonly ITokenConfiguracaoServico _ITokenConfiguracaoServico;
        public RefreshServico( IUsuarioRepositorio usuarioRepositorio, ITokenConfiguracaoServico tokenConfiguracaoServico) 
        {
       
            _IUsuarioRepositorio = usuarioRepositorio;
            _ITokenConfiguracaoServico = tokenConfiguracaoServico;
        }

        public RefreshResponse GerarRefresh(RefreshRequest refreshRequest)
        {
            Usuario? usuario = _IUsuarioRepositorio.ObterPorTokenRefresh(refreshRequest.Refresh);
            if (usuario is null)
            {
                throw new ArgumentException("Acesso não permitido, refresh inválido", nameof(refreshRequest.Refresh));
            }

            //var token = _ITokenConfiguracaoServico.GerarJwtToken(usuario);
            var refresh = _ITokenConfiguracaoServico.GerarRefresh();

            usuario.AtualizaRefresh(refresh);
            _IUsuarioRepositorio.AtualizarAsync(usuario);

            RefreshResponse tokenResponse = new RefreshResponse().ConverteRefresh(refresh);

            return tokenResponse;
        }
    }
}
