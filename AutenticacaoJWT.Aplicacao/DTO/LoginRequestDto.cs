using System.ComponentModel.DataAnnotations;

namespace AutenticacaoJWT.Aplicacao.DTO
{
    public class LoginRequestDto
    {
        //[Required(ErrorMessage = "O campo Login é obrigatório.")]
        public string Login { get; set; } = string.Empty;

        //[Required(ErrorMessage = "O campo Senha é obrigatório.")]
        public string Senha { get; set; } = string.Empty;
    }
}
