namespace AutenticacaoJWT.Aplicacao.Util
{
    public class MinhaExcecaoPersonalizada : Exception
    {
        public MinhaExcecaoPersonalizada(string mensagem) : base(mensagem) { }
    }
}
