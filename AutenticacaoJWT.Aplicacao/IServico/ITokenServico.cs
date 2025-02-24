using AutenticacaoJWT.Aplicacao.Request;
using AutenticacaoJWT.Dominio.Entidade;

namespace AutenticacaoJWT.Aplicacao.IServico
{
    public interface ITokenServico
    {
        Usuario GerarToken(LoginRequest loginRequest);
        Usuario GerarRefreshToken(TokenRequest tokenRequest);
    }
}
