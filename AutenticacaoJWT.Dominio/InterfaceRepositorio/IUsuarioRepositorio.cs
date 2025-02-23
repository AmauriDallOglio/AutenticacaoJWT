using AutenticacaoJWT.Dominio.Entidade;

namespace AutenticacaoJWT.Dominio.InterfaceRepositorio
{
    public interface IUsuarioRepositorio
    {
        void AdicionarUsuario(Usuario novoUsuario);
        List<Usuario> ObterTodosUsuarios();
        Usuario ObterUsuarioPorEmail(string email, string senha);
        bool UsuarioExiste(string email, string senha);
    }
}
