using AutenticacaoJWT.Aplicacao.Request;
using AutenticacaoJWT.Aplicacao.Response;

namespace AutenticacaoJWT.Aplicacao.IServico
{
    public interface ITokenServico
    {
        Task<TokenResponse> GerarToken(LoginRequest loginRequest, CancellationToken cancellationToken);
        //Usuario GerarRefreshToken(TokenRequest tokenRequest);
    }
}
