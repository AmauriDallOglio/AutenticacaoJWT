namespace AutenticacaoJWT.Dominio.Entidade
{
    public class Usuario
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Refresh { get; set; } = string.Empty;
        public string Aplicativo { get; set; } = string.Empty;
        public DateTime UltimoAcesso { get; set; }

        public Usuario() { }

        public void InformaUltimoAcesso()
        {
            UltimoAcesso = DateTime.Now;
            return;
        }

        public Usuario AtualizaTokenRefresh(string token, string refresh)
        {
            Token = token;
            Refresh = refresh;
            InformaUltimoAcesso();
            return this;
        }

    }
}
