namespace AutenticacaoJWT.Aplicacao.DTO
{
    public class UsuarioTokenModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Nome { get; set; }
        public string? Codigo { get; set; }
        public string? Aplicativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? UltimoAcesso { get; set; }
        public string? Refresh { get; set; }
        public string Permissions { get; set; }
    }
}
