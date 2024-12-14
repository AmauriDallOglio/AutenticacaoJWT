# AutenticacaoJWT

Sistema de autenticação baseado em JWT para Single Sign-On (SSO), a ideia principal é permitir que um único processo de autenticação possa ser compartilhado entre várias aplicações (ou sistemas) sem que o usuário precise se autenticar novamente em cada uma delas. 


Primeiro, processo de login e emita o token JWT. Esse servidor será responsável por autenticar o usuário e gerar um JWT válido, que poderá ser usado por outros sistemas.
- O cliente envia o e-mail e senha.
- O servidor valida as credenciais.
- Se as credenciais estiverem corretas, o servidor gera tanto um access token (para autenticação) quanto um refresh token (para renovação do access token) e os envia de volta ao cliente.

Refresh tokens permitir que os usuários permaneçam autenticados mesmo após a expiração do access token. Quando o access token expira, o cliente pode enviar o refresh token para o servidor para obter um novo access token válido.
- Quando o access token expira, o cliente envia o refresh token ao servidor.
- O servidor verifica se o refresh token é válido e corresponde a um usuário.
- Se o refresh token for válido, o servidor gera um novo access token e, opcionalmente, um novo refresh token.
- O novo refresh token é armazenado no lugar do anterior (em memória).



