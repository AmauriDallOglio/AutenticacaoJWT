using AutenticacaoJWT.Dominio.Entidade;

namespace AutenticacaoJWT.Dominio.InterfaceRepositorio
{
    public interface IUsuarioRepositorio
    {
        void AdicionarUsuario(Usuario novoUsuario);
        List<Usuario> ObterTodosUsuarios();
        Task<Usuario?> ObterUsuarioPorEmailSenhaAsync(string email, string senha);
        Usuario? ObterPorTokenRefresh(string refresh);
        Usuario Atualizar(Usuario usuarioAtualizado);

    }
}
