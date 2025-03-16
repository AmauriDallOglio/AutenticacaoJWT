namespace AutenticacaoJWT.Aplicacao.Response
{
    public class RefreshResponse
    {
        public string Refresh { get; set; } = string.Empty;
        
        public RefreshResponse()
        {

        }

        public RefreshResponse ConverteRefresh(string refresh)
        {
            Refresh = refresh;
            return this;
        }
    }
}
