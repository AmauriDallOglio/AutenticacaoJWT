using AutenticacaoJWT.Aplicacao.Request;
using AutenticacaoJWT.Aplicacao.Response;

namespace AutenticacaoJWT.Aplicacao.IServico
{
    public interface IRefreshServico
    {
        RefreshResponse GerarRefresh(RefreshRequest refreshRequest);
    }
}
