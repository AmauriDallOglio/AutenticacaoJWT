using AutenticacaoJWT.Dominio.Entidade;

namespace AutenticacaoJWT.Dominio.InterfaceRepositorio
{
    public interface IUsuarioRepositorio
    {
        void AdicionarUsuario(Usuario novoUsuario);
        List<Usuario> ObterTodosUsuarios();
        Usuario ObterUsuarioPorEmail(string email);
        bool UsuarioExiste(string email);
    }
}
