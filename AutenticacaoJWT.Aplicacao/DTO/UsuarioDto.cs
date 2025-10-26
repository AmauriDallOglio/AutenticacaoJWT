using AutenticacaoJWT.Dominio.Entidade;

namespace AutenticacaoJWT.Aplicacao.DTO
{
    public class UsuarioDto
    {
        public string Nome { get; set; } = "";
        public string Senha { get; set; } = "";
        public string[] Roles { get; set; } = Array.Empty<string>();
        public string[] Versoes { get; set; } = Array.Empty<string>();

        public static List<UsuarioDto> ListaUsuariosDto()
        {
            List<UsuarioDto> lista = new List<UsuarioDto>();
            lista.Add(new UsuarioDto { Nome = "usuario1", Senha = "12345", Roles = new[] { "Usuario" }, Versoes = new[] { "v1" } });
            lista.Add(new UsuarioDto { Nome = "usuario2", Senha = "12345", Roles = new[] { "Usuario" }, Versoes = new[] { "v1", "v2" } });
            lista.Add(new UsuarioDto { Nome = "usuario3", Senha = "12345", Roles = new[] { "Administrador" }, Versoes = new[] { "v1", "v2", "v3" } });

            return lista;
        }
    }
}
