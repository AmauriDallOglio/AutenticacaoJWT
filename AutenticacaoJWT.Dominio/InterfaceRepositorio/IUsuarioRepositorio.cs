using AutenticacaoJWT.Dominio.Entidade;

namespace AutenticacaoJWT.Dominio.InterfaceRepositorio
{
    public interface IUsuarioRepositorio
    {
        public void IncluirAsync(Usuario usuario, CancellationToken cancellationToken);
        public Task<Usuario> AlterarAsync(Usuario usuario, CancellationToken cancellationToken);
        public Task<List<Usuario>> ObterTodosUsuarioAsync(CancellationToken cancellationToken);
        public Task<Usuario?> ObterUsuarioPorEmailSenhaAsync(string email, string senha);
        public Usuario? ObterPorTokenRefresh(string refresh);
 

    }
}
