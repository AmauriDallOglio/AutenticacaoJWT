namespace AutenticacaoJWT.Dominio.Entidade
{
    public class Usuario
    {

        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string? Token { get; set; }
        public string? Codigo { get; set; }
        public string? Aplicativo { get; set; }
        public DateTime? UltimoAcesso { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string? Refresh { get; set; }


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
