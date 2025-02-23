using AutenticacaoJWT.Api.Configuracao;
using AutenticacaoJWT.Aplicacao.Request;
using AutenticacaoJWT.Dominio.Entidade;
using AutenticacaoJWT.Dominio.InterfaceRepositorio;

namespace AutenticacaoJWT.Aplicacao.Servico
{
    public class TokenServico(IUsuarioRepositorio usuarioRepositorio, TokenConfiguracao tokenConfiguracao)
    {
        public readonly IUsuarioRepositorio _IUsuarioRepositorio = usuarioRepositorio;
        public readonly TokenConfiguracao _TokenConfiguracao = tokenConfiguracao;

        public Usuario GerarToken(LoginRequest loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Email))
                throw new ArgumentException("E-mail não informado ", nameof(loginRequest.Email));

            if (string.IsNullOrWhiteSpace(loginRequest.Senha))
                throw new ArgumentException("Senha não informada", nameof(loginRequest.Senha));
            Usuario usuario = _IUsuarioRepositorio.ObterUsuarioPorEmail(loginRequest.Email, loginRequest.Senha);

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

            string token = _TokenConfiguracao.GerarJwtToken(usuario);
            string codigo = _TokenConfiguracao.GerarRefreshToken();
 
            usuario.AtualizaTokenRefresh(token, codigo);

            return  usuario;
        }
    }
}
