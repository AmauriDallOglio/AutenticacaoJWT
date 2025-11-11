using AutenticacaoJWT.Aplicacao.Servico.Interface;
using AutenticacaoJWT.Dominio.Entidade;
using AutenticacaoJWT.Dominio.InterfaceRepositorio;

namespace AutenticacaoJWT.Aplicacao.Controller.Token.RefreshToken
{
    public class RefreshTokenHandler
    {
        public readonly IUsuarioRepositorio _IUsuarioRepositorio;
        public readonly ITokenConfiguracaoServico _ITokenConfiguracaoServico;

        public RefreshTokenHandler(IUsuarioRepositorio usuarioRepositorio, ITokenConfiguracaoServico tokenConfiguracaoServico)
        {
            _IUsuarioRepositorio = usuarioRepositorio;
            _ITokenConfiguracaoServico = tokenConfiguracaoServico;
        }

        public RefreshTokenResponse GerarRefresh(RefreshTokenRequest refreshRequest, CancellationToken cancellationToken)
        {
            Usuario? usuario = _IUsuarioRepositorio.ObterPorTokenRefresh(refreshRequest.Refresh);
            if (usuario is null)
            {
                throw new ArgumentException("Acesso não permitido, refresh inválido", nameof(refreshRequest.Refresh));
            }

            //var token = _ITokenConfiguracaoServico.GerarJwtToken(usuario);
            var refresh = _ITokenConfiguracaoServico.GerarRefresh();

            usuario.AtualizaRefresh(refresh);
            _IUsuarioRepositorio.AlterarAsync(usuario, cancellationToken);

            RefreshTokenResponse tokenResponse = new RefreshTokenResponse().ConverteRefresh(refresh);

            return tokenResponse;
        }
    }
}
