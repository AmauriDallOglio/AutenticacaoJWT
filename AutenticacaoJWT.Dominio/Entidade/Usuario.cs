namespace AutenticacaoJWT.Dominio.Entidade
{
    public class Usuario
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public string Aplicativo { get; set; } = string.Empty;
        public DateTime UltimoAcesso { get; set; }

    }
}
