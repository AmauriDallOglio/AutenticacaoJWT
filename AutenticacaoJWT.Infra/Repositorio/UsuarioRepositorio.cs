using AutenticacaoJWT.Dominio.Entidade;
using AutenticacaoJWT.Dominio.InterfaceRepositorio;

namespace AutenticacaoJWT.Infra.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly List<Usuario> _usuarios;

        public UsuarioRepositorio()
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
                    Codigo = "",
                    Aplicativo = ""
                },
                new Usuario
                {
                    Id = Guid.NewGuid().ToString(),
                    Nome = "Amauri2",
                    Email = "amauri2@amauri.com",
                    Senha = "123456",
                    Token = "",
                    Codigo = "",
                    Aplicativo = ""
                }
            };
        }

        public void AdicionarUsuario(Usuario novoUsuario)
        {
            if (!UsuarioExiste(novoUsuario.Email, novoUsuario.Senha))
            {
                _usuarios.Add(novoUsuario);
            }
            else
            {
                throw new Exception($"Usuário com o e-mail {novoUsuario.Email} já existe.");
            }
        }

        public List<Usuario> ObterTodosUsuarios()
        {
            return _usuarios;
        }

        public Usuario ObterUsuarioPorEmail(string email, string senha)
        {
            var usuario = _usuarios.FirstOrDefault(u => u.Email == email && u.Senha == senha);
            if (usuario == null)
                throw new Exception($"Usuário com o e-mail {email} não encontrado.");

            return usuario;
        }

        public bool UsuarioExiste(string email, string senha)
        {
            return _usuarios.Any(u => u.Email == email && u.Senha == senha);
        }
    }
}
