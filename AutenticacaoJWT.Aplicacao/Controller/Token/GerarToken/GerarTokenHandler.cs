using AutenticacaoJWT.Aplicacao.Servico.Interface;
using AutenticacaoJWT.Dominio.Entidade;
using AutenticacaoJWT.Dominio.InterfaceRepositorio;

namespace AutenticacaoJWT.Aplicacao.Controller.Token.GerarToken
{
    public class GerartokenHandler
    {
        public readonly IGenericoRepositorio<Usuario> _IGenericoRepositorioUsuario;
        public readonly IUsuarioRepositorio _IUsuarioRepositorio;
        public readonly ITokenConfiguracaoServico _ITokenConfiguracaoServico;

        public GerartokenHandler(IGenericoRepositorio<Usuario> iGenericoRepositorioUsuario, IUsuarioRepositorio iUsuarioRepositorio, ITokenConfiguracaoServico iTokenConfiguracaoServico)
        {
            _IGenericoRepositorioUsuario = iGenericoRepositorioUsuario;
            _IUsuarioRepositorio = iUsuarioRepositorio;
            _ITokenConfiguracaoServico = iTokenConfiguracaoServico;
        }

        public async Task<GerarTokenResponse> GerarToken(GerarTokenRequest loginRequest, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Email))
                throw new ArgumentException("E-mail não informado!", nameof(loginRequest.Email));

            if (string.IsNullOrWhiteSpace(loginRequest.Senha))
                throw new ArgumentException("Senha não informada!", nameof(loginRequest.Senha));

            Usuario? usuario = await _IUsuarioRepositorio.ObterUsuarioPorEmailSenhaAsync(loginRequest.Email, loginRequest.Senha);
            if (usuario is null)
            {
                throw new ArgumentException("Usuário inválido ", nameof(loginRequest.Email));
            }
            else
            {
                if (usuario.Equals(loginRequest.Senha))
                {
                    throw new ArgumentException("Acesso não permitido ", nameof(loginRequest.Email));
                }
            }

            string refresh = _ITokenConfiguracaoServico.GerarRefresh();
            usuario.Refresh = refresh;
            string token = _ITokenConfiguracaoServico.GerarJwtToken(usuario);


            usuario.AtualizaTokenRefresh(token, refresh);

            await _IGenericoRepositorioUsuario.EditarAsync(usuario, cancellationToken);

            GerarTokenResponse tokenResponse = new GerarTokenResponse().ConverteUsuario(usuario);
            return tokenResponse;
        }

    }
}
