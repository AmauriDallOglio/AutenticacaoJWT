using AutenticacaoJWT.Aplicacao.Request;
using AutenticacaoJWT.Dominio.Entidade;

namespace AutenticacaoJWT.Aplicacao.IServico
{
    public interface ITokenServico
    {
        Task<Usuario> GerarToken(LoginRequest loginRequest, CancellationToken cancellationToken);
        Usuario GerarRefreshToken(TokenRequest tokenRequest);
    }
}
