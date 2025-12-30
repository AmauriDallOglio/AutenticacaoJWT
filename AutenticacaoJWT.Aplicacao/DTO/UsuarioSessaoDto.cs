using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutenticacaoJWT.Aplicacao.DTO
{
    public class UsuarioSessaoDto
    {
        public Guid IdUsuario { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Aplicativo { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; }
        public DateTime? UltimoAcesso { get; set; }
        public string[] Permissoes { get; set; } = Array.Empty<string>();
    }
}
