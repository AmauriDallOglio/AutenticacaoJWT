namespace AutenticacaoJWT.Api.Configuracao
{
    public class HelperConsoleColor
    {

        public static void Padrao(string menssagem, ConsoleColor foreground, ConsoleColor background)
        {
            Console.BackgroundColor = background;
            Console.ForegroundColor = foreground;
            Console.WriteLine(menssagem);
            Console.ResetColor(); // Restaura as cores padrão após a mensagem.
        }


        public static void Erro(string menssagem)
        {
            Padrao($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {menssagem}", ConsoleColor.White, ConsoleColor.Red);
        }


        public static void Sucesso(string menssagem)
        {
            Padrao($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {menssagem}", ConsoleColor.Black, ConsoleColor.Green);
        }


        public static void Alerta(string menssagem)
        {
            Padrao($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {menssagem}", ConsoleColor.Black, ConsoleColor.Yellow);
        }

        public static void Info(string menssagem)
        {
            Padrao($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {menssagem}", ConsoleColor.Yellow, ConsoleColor.Blue);
        }

        public static void Teste()
        {
            Erro("Erro crítico: Arquivo não encontrado!");
            Sucesso("Operação concluída com sucesso!");
            Alerta("Carregando configurações do sistema...");
            Info("Carregando configurações do sistema...");
            Padrao("Mensagem personalizada!", ConsoleColor.Cyan, ConsoleColor.Black);
        }
    }
}