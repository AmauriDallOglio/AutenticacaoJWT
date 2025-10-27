namespace AutenticacaoJWT.Aplicacao.DTO
{
    public class UsuarioToken
    {
        public string Email { get; set; }
        public string Nome { get; set; }
        public string Refresh { get; set; }
        public string Permissions { get; set; }
    }
}
