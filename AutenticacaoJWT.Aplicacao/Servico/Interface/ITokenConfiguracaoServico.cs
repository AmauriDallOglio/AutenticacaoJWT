using AutenticacaoJWT.Dominio.Entidade;

namespace AutenticacaoJWT.Aplicacao.Servico.Interface
{
    public interface ITokenConfiguracaoServico
    {
        byte[] CodigoChave();
        string CodigoSecret();
        string GerarJwtToken(Usuario usuario);
        string GerarRefresh();

    }
}
