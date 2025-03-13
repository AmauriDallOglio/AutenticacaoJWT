using AutenticacaoJWT.Dominio.Entidade;

namespace AutenticacaoJWT.Aplicacao.Response
{
    public class TokenResponse
    {
 
        public string Token { get; set; } = string.Empty;
        public string Refresh { get; set; } = string.Empty;
        public string Aplicativo { get; set; } = string.Empty;
        
        public TokenResponse ConverteUsuario(Usuario usuario)
        {
            this.Token = usuario.Token;
            this.Refresh = usuario.Refresh;
            this.Aplicativo = usuario.Aplicativo;
            return this;
        }
    }
}
