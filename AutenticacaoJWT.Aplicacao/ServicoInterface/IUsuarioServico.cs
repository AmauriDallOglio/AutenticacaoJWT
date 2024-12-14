namespace AutenticacaoJWT.Aplicacao.ServicoInterface
{
    public interface IUsuarioServico
    {
        bool ValidarCredenciais(string email, string password);
        Dictionary<string, string> ObterUsuarios();
    }
}
