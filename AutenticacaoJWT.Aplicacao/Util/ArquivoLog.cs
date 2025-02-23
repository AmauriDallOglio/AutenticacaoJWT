namespace AutenticacaoJWT.Aplicacao.Util
{
    public class ArquivoLog
    {
        public async Task<string> IncluirLinha(string caminhoNomeArquivo, Exception ex, string requestPath, string mensagemBasica)
        {
            string separador = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} -----------------------------------------------------------------------------{Environment.NewLine}";
            string mensagemRetorno = $"TratamentoErroMiddleware | Path: {requestPath} | {mensagemBasica}: {ex.Message}{Environment.NewLine}";

            var mensagemPersonalizada = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {mensagemRetorno}{Environment.NewLine}";

            File.AppendAllText(caminhoNomeArquivo, separador);
            File.AppendAllText(caminhoNomeArquivo, mensagemPersonalizada);

            Console.WriteLine($"Erro: {mensagemPersonalizada}");

            return mensagemRetorno;
        }
    }
}

