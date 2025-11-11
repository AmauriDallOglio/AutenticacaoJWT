namespace AutenticacaoJWT.Aplicacao.DTO
{
    public class UsuarioToken
    {
        public string Email { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Refresh { get; set; } = string.Empty;
        public string Permissions { get; set; } = string.Empty;
    }
}
