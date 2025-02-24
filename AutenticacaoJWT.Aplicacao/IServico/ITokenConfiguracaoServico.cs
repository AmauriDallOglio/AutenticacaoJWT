using AutenticacaoJWT.Dominio.Entidade;

namespace AutenticacaoJWT.Aplicacao.IServico
{
    public interface ITokenConfiguracaoServico
    {
        byte[] CodigoChave();
        string CodigoSecret();
        string GerarJwtToken(Usuario usuario);
        string GerarRefreshToken();

    }
}
