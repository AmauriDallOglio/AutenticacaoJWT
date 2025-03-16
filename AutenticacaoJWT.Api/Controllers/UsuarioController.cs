using AutenticacaoJWT.Dominio.Entidade;
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

        //[HttpPost("Adicionar")]
        //public IActionResult AdicionarUsuario([FromBody] Usuario novoUsuario)
        //{
        //    try
        //    {
        //        _usuarioRepositorio.AdicionarUsuario(novoUsuario);
        //        return Ok(new { Message = "Usuário adicionado com sucesso!" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { Error = ex.Message });
        //    }
        //}

        //[HttpGet("ObterTodos")]
        //public IActionResult ObterTodosUsuarios()
        //{
        //    return Ok(_usuarioRepositorio.ObterTodosUsuarios());
        //}
    }

}
