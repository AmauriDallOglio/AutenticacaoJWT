namespace AutenticacaoJWT.Aplicacao.Util
{
    public class ArquivoLog
    {
        public async Task<string> IncluirLinha(string caminhoNomeArquivo, Exception ex, string requestPath, string mensagemBasica)
        {
            string mensagemRetorno = $"TratamentoErroMiddleware | Path: {requestPath} | {mensagemBasica}: {ex.Message} {Environment.NewLine}";

            var mensagemPersonalizada = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {mensagemRetorno}";
            File.AppendAllText(caminhoNomeArquivo, mensagemPersonalizada);
            Console.WriteLine($"Erro: {mensagemPersonalizada}");

            return mensagemRetorno;
        }
    }
}

