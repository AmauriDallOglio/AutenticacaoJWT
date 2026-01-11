using AutenticacaoJWT.Dominio.InterfaceRepositorio;
using Microsoft.AspNetCore.Mvc;

namespace AutenticacaoJWT.Api.Controllers
{

    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public UsuarioController(IUsuarioRepositorio usuarioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
        }

        [HttpGet("PerfilLogado")]
        public IActionResult PerfilLogado()
        {
            var user = HttpContext.User;

            var id = user.FindFirst("Id")?.Value;
            var email = user.FindFirst("Email")?.Value;
            var nome = user.FindFirst("Nome")?.Value;
            var permissoes = user.FindFirst("Permissoes")?.Value;

            return Ok(new
            {
                Id = id,
                Email = email,
                Nome = nome,
                Permissoes = permissoes
            });
        }
    }
}
