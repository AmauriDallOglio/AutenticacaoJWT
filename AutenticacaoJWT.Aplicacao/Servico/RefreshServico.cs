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

            var token = _ITokenConfiguracaoServico.GerarJwtToken(usuario);
            var codigo = _ITokenConfiguracaoServico.GerarRefresh();

            usuario.AtualizaTokenRefresh(token, codigo);
            RefreshResponse tokenResponse = new RefreshResponse() { Refresh = usuario.Refresh };

            return tokenResponse;
        }
    }
}
