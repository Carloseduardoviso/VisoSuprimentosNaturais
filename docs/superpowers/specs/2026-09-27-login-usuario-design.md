# Login por CPF com tabela Usuario

## Objetivo

Substituir o fluxo de login baseado em ASP.NET Identity por uma tabela própria `Usuario`, usando CPF e senha. O sistema deve exigir autenticação para acessar as áreas protegidas e encerrar a sessão após seis horas.

## Modelo de dados

`Usuario` terá identificador interno, `NomeCompleto`, CPF normalizado e único, `SenhaHash`, indicador de ativo e datas de criação/atualização. A senha nunca será armazenada em texto puro; a verificação usará o hasher seguro do ASP.NET Core.

## Fluxo de autenticação

- `/Conta/Entrar` exibirá CPF e senha.
- CPF será normalizado removendo pontuação e espaços antes da consulta.
- Login válido emitirá um cookie de autenticação próprio, com expiração absoluta de seis horas e sem renovação além desse limite.
- Usuário inativo, CPF inexistente ou senha inválida receberão resposta genérica.
- Rotas protegidas exigirão autenticação; expirado o cookie, o middleware redirecionará para a tela de login.
- Logout invalidará o cookie.

## Cadastro interno

Uma tela de cadastro sem link no menu ficará disponível em rota específica para criação de usuários. O acesso exigirá uma chave de cadastro configurada somente no ambiente do servidor (`USER_REGISTRATION_KEY`); a chave não será gravada no Git nem exibida em páginas. O cadastro validará nome, CPF, senha forte e confirmação de senha, além de impedir CPF duplicado.

## Remoção do Identity

O novo fluxo não usará `UserManager`, `SignInManager`, `IdentityDbContext`, tabelas `AspNetUsers` ou papéis do Identity. As políticas de autorização existentes serão reduzidas a autenticação enquanto o requisito atual é somente login; permissões por perfil ficam fora deste escopo.

## Migração e testes

Será criada uma migração para `Usuario`, testes unitários para normalização de CPF, hash/verificação e expiração, além de testes de integração para login, logout, cadastro protegido e redirecionamento de sessão expirada. O banco de produção atual será preservado; nenhuma tabela existente será apagada automaticamente.
