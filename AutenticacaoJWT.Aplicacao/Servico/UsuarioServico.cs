using AutenticacaoJWT.Aplicacao.ServicoInterface;

namespace AutenticacaoJWT.Aplicacao.Servico
{
    public class UsuarioServico : IUsuarioServico
    {
        public bool ValidarCredenciais(string email, string senha)
        {

            var usuarios = ObterUsuarios();
            return usuarios.ContainsKey(email) && usuarios[email] == senha;
        }

        public Dictionary<string, string> ObterUsuarios()
        {
            return new()
            {
                { "amauri1@amauri.com", "amauri123" },
                { "amauri2@amauri.com", "amauri123" }
            };
        }
    }
}
