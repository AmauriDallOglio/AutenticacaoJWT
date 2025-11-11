using AutenticacaoJWT.Dominio.Entidade;

namespace AutenticacaoJWT.Aplicacao.Servico.Interface
{
    public interface ITokenConfiguracaoServico
    {
        public byte[] CodigoChave();
        public string CodigoSecret();
        public string GerarJwtToken(Usuario usuario);
        public string GerarRefresh();

    }
}
