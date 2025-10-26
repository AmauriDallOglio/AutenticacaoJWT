namespace AutenticacaoJWT.Aplicacao.Controller.Token.RefreshToken
{
    public class RefreshTokenResponse
    {
        public string Refresh { get; set; } = string.Empty;

        public RefreshTokenResponse()
        {

        }

        public RefreshTokenResponse ConverteRefresh(string refresh)
        {
            Refresh = refresh;
            return this;
        }
    }
}
