# VISO ERP

Sistema comercial da VISO SUPRIMENTOS NATURAIS em .NET 10, ASP.NET Core MVC, EF Core 10 e SQL Server.

## Solução

Abra `VisoERP.slnx`. As pastas aparecem nesta ordem: `Web`, `Application`, `Domain`, `Infra` e `Testes`. O projeto `VisoSuprimentosNaturais.slnx` preexistente permanece fora da nova solução. O sistema não inclui API.

Fluxo planejado: Controller → Application Service → contrato de repositório em Domain → implementação em Infra.Data → SQL Server. A configuração das dependências fica em `VisoERP.Infra.Ioc/Modulo.cs`; o DbContext existe somente em `VisoERP.Infra.Data/Context`.

## Desenvolvimento local

Pré-requisitos: SDK .NET 10, SQL Server LocalDB ou SQL Server, e ferramenta `dotnet-ef` 10.

```powershell
dotnet restore VisoERP.slnx
dotnet build VisoERP.slnx
dotnet ef database update --project VisoERP.Infra.Data --startup-project VisoERP.Web --context VisoErpDbContext
dotnet run --project VisoERP.Web
dotnet test VisoERP.slnx
```

O ambiente Development usa LocalDB no banco `VisoERP`. Para outro servidor, defina `ConnectionStrings__VisoERP` no ambiente ou em secrets locais. Produção exige configuração própria; não mantenha credenciais no repositório.

AutoMapper 16 usa licença; forneça `AutoMapper__LicenseKey` por configuração segura antes da publicação. Os perfis ficam em `VisoERP.Application/AutoMapper`.

### Acesso temporário em desenvolvimento

`VisoERP.Web/appsettings.Development.json` habilita `Authentication:AllowAnonymousDevelopment`. Com isso, a aplicação local não solicita login, inclusive no cadastro de suprimentos. Essa opção só tem efeito quando `ASPNETCORE_ENVIRONMENT=Development`; em outros ambientes, o login e as funções são exigidos. Remova ou desative a opção antes de testar permissões.

### Primeiro administrador

Após aplicar as migrations, configure um e-mail e uma senha forte com `dotnet user-secrets` no projeto Web:

```powershell
dotnet user-secrets set --project VisoERP.Web "AdminBootstrap:Email" "admin@exemplo.com"
dotnet user-secrets set --project VisoERP.Web "AdminBootstrap:Password" "SENHA_FORTE_ESCOLHIDA_PELO_ADMINISTRADOR"
```

Inicie a aplicação uma vez. O usuário é criado com a função `Administrador`; as senhas são armazenadas pelo Identity como hash. Depois remova as duas chaves de bootstrap dos secrets. Não há auto cadastro público. Funções previstas: `Administrador`, `Estoque`, `Vendas`, `Financeiro`.

## Estado de implementação

- Etapa 1: solução, referências, persistência, DI e migration inicial.
- Etapa 2: Identity, login, funções, cadastro de suprimentos e migration `IdentidadeESuprimentos`.
- Etapa 3: categorias, clientes e fornecedores, com telas de listagem/edição e migration `ClientesEFornecedores`.
- Etapas 4 a 8: pendentes.

As migrations ficam em `VisoERP.Infra.Data/Migrations` e devem ser revisadas antes de aplicação em outros ambientes. O banco LocalDB é apenas para desenvolvimento.
