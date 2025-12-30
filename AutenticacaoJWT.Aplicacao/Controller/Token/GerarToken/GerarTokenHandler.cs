using AutenticacaoJWT.Aplicacao.Servico.Interface;
using AutenticacaoJWT.Dominio.Entidade;
using AutenticacaoJWT.Dominio.InterfaceRepositorio;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AutenticacaoJWT.Aplicacao.Controller.Token.GerarToken
{
    public class GerartokenHandler
    {
        public readonly IGenericoRepositorio<Usuario> _IGenericoRepositorioUsuario;
        public readonly IUsuarioRepositorio _IUsuarioRepositorio;
        public readonly ITokenConfiguracaoServico _ITokenConfiguracaoServico;

        public GerartokenHandler(IGenericoRepositorio<Usuario> iGenericoRepositorioUsuario, IUsuarioRepositorio iUsuarioRepositorio, ITokenConfiguracaoServico iTokenConfiguracaoServico)
        {
            _IGenericoRepositorioUsuario = iGenericoRepositorioUsuario;
            _IUsuarioRepositorio = iUsuarioRepositorio;
            _ITokenConfiguracaoServico = iTokenConfiguracaoServico;
        }

        public async Task<GerarTokenResponse> GerarToken(GerarTokenRequest loginRequest, CancellationToken cancellationToken)
        {

            if (string.IsNullOrWhiteSpace(loginRequest.Email))
                throw new ArgumentException("E-mail não informado!", nameof(loginRequest.Email));

            if (string.IsNullOrWhiteSpace(loginRequest.Senha))
                throw new ArgumentException("Senha não informada!", nameof(loginRequest.Senha));

            Usuario? usuario = await _IUsuarioRepositorio.ObterUsuarioPorEmailSenhaAsync(loginRequest.Email, loginRequest.Senha);
            if (usuario is null)
                throw new ArgumentException("Usuário inválido", nameof(loginRequest.Email));

            string refresh = _ITokenConfiguracaoServico.GerarRefresh();
            usuario.Refresh = refresh;

            // Chave secreta
            var key = Encoding.UTF8.GetBytes("minha_chave_secreta_super_segura");

            // Claims com dados completos
            var claims = new List<Claim>
            {
                new Claim("Id", usuario.Id.ToString()),
                new Claim("Email", usuario.Email),
                new Claim("Nome", usuario.Nome),
                new Claim("Refresh", usuario.Refresh ?? ""),
                new Claim("Codigo", usuario.Codigo ?? ""),
                new Claim("Aplicativo", usuario.Aplicativo ?? ""),
                new Claim("DataCadastro", usuario.DataCadastro.ToString("o")), // formato ISO 8601
                new Claim("UltimoAcesso", usuario.UltimoAcesso?.ToString("o") ?? ""),
                new Claim("Permissoes", "read,write") // exemplo de permissão
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                NotBefore = DateTime.UtcNow,
                IssuedAt = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            usuario.AtualizaTokenRefresh(token, refresh);
            await _IGenericoRepositorioUsuario.EditarAsync(usuario, cancellationToken);

            GerarTokenResponse tokenResponse = new GerarTokenResponse().ConverteUsuario(usuario);
            return tokenResponse;

        }

    }
}



//if (string.IsNullOrWhiteSpace(loginRequest.Email))
//    throw new ArgumentException("E-mail não informado!", nameof(loginRequest.Email));

//if (string.IsNullOrWhiteSpace(loginRequest.Senha))
//    throw new ArgumentException("Senha não informada!", nameof(loginRequest.Senha));

//Usuario? usuario = await _IUsuarioRepositorio.ObterUsuarioPorEmailSenhaAsync(loginRequest.Email, loginRequest.Senha);
//if (usuario is null)
//{
//    throw new ArgumentException("Usuário inválido ", nameof(loginRequest.Email));
//}
//else
//{
//    if (usuario.Equals(loginRequest.Senha))
//    {
//        throw new ArgumentException("Acesso não permitido ", nameof(loginRequest.Email));
//    }
//}

//string refresh = _ITokenConfiguracaoServico.GerarRefresh();
//usuario.Refresh = refresh;
//string token = _ITokenConfiguracaoServico.GerarJwtToken(usuario);


//usuario.AtualizaTokenRefresh(token, refresh);

//await _IGenericoRepositorioUsuario.EditarAsync(usuario, cancellationToken);

//GerarTokenResponse tokenResponse = new GerarTokenResponse().ConverteUsuario(usuario);
//return tokenResponse;