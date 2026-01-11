namespace AutenticacaoJWT.Dominio.Entidade
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string? Token { get; set; } 
        public string? Codigo { get; set; }
        public string? Aplicativo { get; set; }
        public DateTime? UltimoAcesso { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string? Refresh { get; set; }

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

        public Usuario AtualizaRefresh( string refresh)
        {
 
            Refresh = refresh;
            InformaUltimoAcesso();
            return this;
        }
    }
}
