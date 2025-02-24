using AutenticacaoJWT.Dominio.Entidade;
using AutenticacaoJWT.Dominio.InterfaceRepositorio;

namespace AutenticacaoJWT.Infra.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly List<Usuario> _usuarios;

        public UsuarioRepositorio()
        {
            if (_usuarios == null)
            {
                _usuarios = new List<Usuario>
                {
                    new Usuario
                    {
                        Id = Guid.NewGuid().ToString(),
                        Nome = "Amauri1",
                        Email = "amauri1@amauri.com",
                        Senha = "123456",
                        Token = "",
                        Refresh = "",
                        Aplicativo = ""
                    },
                    new Usuario
                    {
                        Id = Guid.NewGuid().ToString(),
                        Nome = "Amauri2",
                        Email = "amauri2@amauri.com",
                        Senha = "123456",
                        Token = "",
                        Refresh = "",
                        Aplicativo = ""
                    }
                };
            }
        }

        public void AdicionarUsuario(Usuario novoUsuario)
        {
  
                _usuarios.Add(novoUsuario);

        }

        public List<Usuario> ObterTodosUsuarios()
        {
            return _usuarios;
        }

        public Usuario ObterUsuarioPorEmailSenha(string email, string senha)
        {
            var usuario = _usuarios.FirstOrDefault(u => u.Email == email && u.Senha == senha);
            if (usuario == null)
                throw new Exception($"Usuário com o e-mail {email} não encontrado.");

            return usuario;
        }

        public Usuario? ObterPorTokenRefresh(string refresh)
        {
            Usuario? usuario = _usuarios.FirstOrDefault(u => u.Refresh == refresh);
            return usuario;
        }


        public Usuario Atualizar(Usuario usuarioAtualizado)
        {
            var usuario = _usuarios.FirstOrDefault(u => u.Id == usuarioAtualizado.Id);
            if (usuario != null)
            {
                usuario.Nome = usuarioAtualizado.Nome;
                usuario.Email = usuarioAtualizado.Email;
                usuario.Senha = usuarioAtualizado.Senha;
                usuario.Token = usuarioAtualizado.Token;
                usuario.Refresh = usuarioAtualizado.Refresh;
                usuario.Aplicativo = usuarioAtualizado.Aplicativo;
            }
            return usuario;
        }

    }
}
