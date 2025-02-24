﻿using AutenticacaoJWT.Aplicacao.IServico;
using AutenticacaoJWT.Aplicacao.Request;
using AutenticacaoJWT.Dominio.Entidade;
using AutenticacaoJWT.Dominio.InterfaceRepositorio;

namespace AutenticacaoJWT.Aplicacao.Servico
{
    public class TokenServico(IUsuarioRepositorio usuarioRepositorio, ITokenConfiguracaoServico iTokenConfiguracaoServico) : ITokenServico
    {
        public readonly IUsuarioRepositorio _IUsuarioRepositorio = usuarioRepositorio;
        public readonly ITokenConfiguracaoServico _ITokenConfiguracaoServico = iTokenConfiguracaoServico;

        public Usuario GerarToken(LoginRequest loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Email))
                throw new ArgumentException("E-mail não informado ", nameof(loginRequest.Email));

            if (string.IsNullOrWhiteSpace(loginRequest.Senha))
                throw new ArgumentException("Senha não informada", nameof(loginRequest.Senha));

            Usuario usuario = _IUsuarioRepositorio.ObterUsuarioPorEmailSenha(loginRequest.Email, loginRequest.Senha);

            if (usuario is null)
            {
                throw new ArgumentException("Usuário inválido ", nameof(loginRequest.Email));
            }
            else
            {
                if (usuario.Equals(loginRequest.Senha))
                {
                    throw new ArgumentException("Acesso não permitido ", nameof(loginRequest.Email));
                }
            }

            string token = _ITokenConfiguracaoServico.GerarJwtToken(usuario);
            string refresh = _ITokenConfiguracaoServico.GerarRefreshToken();
 
            usuario.AtualizaTokenRefresh(token, refresh);

            _IUsuarioRepositorio.Atualizar(usuario);

            return  usuario;
        }


        public Usuario GerarRefreshToken(TokenRequest tokenRequest)
        {
            Usuario? usuario = _IUsuarioRepositorio.ObterPorTokenRefresh(tokenRequest.Refresh);
            if (usuario is null)
            {
                throw new ArgumentException("Acesso não permitido, token inválido", nameof(tokenRequest.Refresh));
          
            }

            var token = _ITokenConfiguracaoServico.GerarJwtToken(usuario);
            var codigo = _ITokenConfiguracaoServico.GerarRefreshToken();

            usuario.AtualizaTokenRefresh(token, codigo);
            return usuario;
        }
    }
}
