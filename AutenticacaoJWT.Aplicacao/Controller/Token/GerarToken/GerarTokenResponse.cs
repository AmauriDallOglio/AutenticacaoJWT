using AutenticacaoJWT.Dominio.Entidade;

namespace AutenticacaoJWT.Aplicacao.Controller.TokenController.GerarToken
{
    public class GerarTokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Refresh { get; set; } = string.Empty;
        public string Aplicativo { get; set; } = string.Empty;

        public GerarTokenResponse ConverteUsuario(Usuario usuario)
        {
            this.Token = usuario.Token;
            this.Refresh = usuario.Refresh;
            this.Aplicativo = usuario.Aplicativo;
            return this;
        }
    }
}
