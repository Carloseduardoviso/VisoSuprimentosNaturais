# VISO ERP

Sistema comercial da VISO SUPRIMENTOS NATURAIS em .NET 10, ASP.NET Core MVC, EF Core 10 e SQL Server.

## Identidade visual

O tema usa verde escuro `#143d2b`, dourado `#c8a963` e branco. As cores e os componentes compartilhados ficam em `VisoERP.Web/wwwroot/css/site.css`. O símbolo da folha é um SVG local em `wwwroot/images/viso-leaf.svg`; a página inicial e o acesso exibem a marca VISO Suprimentos Naturais. Slogan: “O melhor da natureza para você.”

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
- Etapa 4: pedidos a fornecedores, recebimento parcial, entradas diretas com cartões dinâmicos, lote, validade, custo médio móvel, saldo e movimentações. A confirmação usa transação serializável e `rowversion` no saldo/lote. Migration `PedidosEEntradas`.
- Etapa 5: vendas com múltiplos itens, baixa transacional do estoque por lote válido, custo histórico, entrada financeira, parcelamento de até 12 vezes e recebimentos parciais de parcelas. Migration `VendasEParcelas`.
- Etapa 6: investimentos, despesas por competência, contas a pagar geradas na confirmação das entradas, pagamentos parciais e resumo financeiro por período. Migration `FinanceiroEInvestimentos` inclui contas das entradas anteriores à etapa.
- Etapas 7 e 8: pendentes (dashboard e relatórios com exportação; auditoria e preparo de publicação).

As migrations ficam em `VisoERP.Infra.Data/Migrations` e devem ser revisadas antes de aplicação em outros ambientes. O banco LocalDB é apenas para desenvolvimento.

O teste de integração `PedidoEntradaIntegracaoTests` cria um banco LocalDB temporário com nome exclusivo, aplica as migrations, confirma recebimentos parciais e remove somente esse banco de teste ao terminar. Execute-o em um contexto com acesso ao LocalDB.

O resumo financeiro separa vendas realizadas de recebimentos. O custo dos produtos vendidos usa o custo médio registrado na venda. O lucro líquido exibido é uma estimativa de vendas menos CPV e despesas cadastradas; tributos, devoluções e outras provisões ainda não estão modelados. O ROI divide esse lucro pelo capital investido acumulado até o fim do período, quando houver capital cadastrado.
