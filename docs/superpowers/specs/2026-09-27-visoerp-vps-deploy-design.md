# Design: publicação do VisoERP na VPS

Data: 2026-09-27  
Status: desenho aprovado em conversa; aguardando revisão deste documento.

## Objetivo e critérios de sucesso

Publicar o VisoERP em `visosuplementosnaturais.com.br` na VPS Contabo existente, com a aplicação web e o banco de dados em contêineres separados. A primeira inicialização deve usar um banco vazio, sem importar os dados do LocalDB. O site deve estar acessível por HTTPS; o SQL Server não deve ficar acessível pela internet.

O domínio foi registrado por R$ 40 para um ano e aparece como publicado no Registro.br, com validade até 27/09/2027. O pagamento foi feito pelo titular e a ativação foi conferida no painel.

## Abordagens consideradas

1. **Escolhida — dois contêineres e Caddy instalado no host.** Docker Compose gerencia os contêineres `web` e `db`; Caddy, instalado como serviço da VPS, recebe tráfego público, obtém/renova certificado HTTPS e encaminha requisições à aplicação. Mantém exatamente os dois contêineres solicitados.
2. **Caddy como terceiro contêiner.** Mantém todo o stack no Compose, mas adiciona um serviço além dos contêineres web e banco solicitados.
3. **Publicar a aplicação diretamente.** Menos peças, porém deixa a configuração de TLS, certificados e renovação a cargo de outro mecanismo; não é a opção recomendada.

## Arquitetura aprovada

- Aplicação ASP.NET Core MVC em imagem multi-stage baseada em .NET 10, servida internamente na porta 8080. O projeto já usa `net10.0` e ASP.NET Core; .NET 10 é LTS com suporte até novembro de 2028 ([ciclo oficial do .NET](https://learn.microsoft.com/en-us/dotnet/core/releases-and-support)).
- Banco em contêiner Linux `mcr.microsoft.com/mssql/server:2025-latest`, configurado com `MSSQL_PID=Express`. SQL Server Express é a edição gratuita apropriada para aplicação pequena; no SQL Server 2025, o limite relacional é 50 GB ([edições no Linux](https://learn.microsoft.com/en-us/sql/linux/sql-server-linux-editions-and-components-2025), [limites da edição](https://learn.microsoft.com/en-us/sql/sql-server/editions-and-components-of-sql-server-2025)). A tag escolhida recebe atualizações da linha 2025; atualizações devem ser aplicadas de forma controlada e após backup.
- Os serviços `web` e `db` compartilham uma rede privada do Compose. A aplicação conecta ao host interno `db`; a porta 1433 não será publicada no host.
- Os arquivos do banco ficam em volume persistente montado em `/var/opt/mssql`, para sobreviver à substituição do contêiner.
- Caddy roda no host como serviço, com proxy para `127.0.0.1`/porta publicada localmente pela aplicação. Ele atende HTTP/HTTPS e administra certificados TLS. O tráfego para a aplicação não será publicado em interface externa.
- DNS do domínio raiz e, se utilizado, `www` apontará para o IP público da VPS. A configuração exata depende dos registros e nameservers atuais no Registro.br.
- Firewall do host permitirá somente SSH administrativo e portas 80/443 públicas. O acesso SSH será restrito e a porta do banco permanecerá fechada externamente.

## Configuração, dados e segredos

- Produção usará uma connection string `ConnectionStrings__VisoERP` apontando para `db`; não usará LocalDB.
- Configurações sensíveis ficarão apenas na VPS, em arquivo de ambiente com permissões restritas ou mecanismo equivalente. Nunca serão adicionadas ao Git. Incluem senha SQL, `AutoMapper__LicenseKey` e configuração temporária do primeiro administrador.
- Aplicar as migrations existentes do EF Core ao banco vazio como etapa explícita do deploy, após revisar a saída e fazer backup antes de futuras atualizações. Não importar dados locais.
- Criar o primeiro usuário `Administrador` usando `AdminBootstrap:Email` e `AdminBootstrap:Password`, conforme o fluxo documentado no README. Remover esses dois segredos de bootstrap depois que o usuário for criado. Não habilitar cadastro anônimo em produção.
- O titular define a senha do administrador por canal seguro; credenciais não serão solicitadas ou transmitidas em texto aberto pelo chat.

## Operação, falhas e recuperação

- Política de reinício e health checks nos dois serviços; a aplicação só será considerada saudável depois de conseguir atender requisições, e o banco deverá estar pronto antes de a conexão ser validada.
- Backup diário do banco para diretório persistente na VPS, com retenção definida no plano de implementação. Esse backup protege contra falhas de contêiner, mas não contra perda da VPS. Cópia fora da VPS é recomendada antes de inserir dados comerciais reais, mas não será contratada/configurada sem escolha de destino pelo titular.
- Antes de atualizar imagens ou migrations, gerar backup e validar a imagem. Se a versão web falhar, voltar à imagem anterior. Rollback de alterações de schema não será automático; será feito por restauração do backup se necessário.
- Se DNS, emissão de certificado ou conexão com SQL falhar, manter serviços internos sem expor o banco, inspecionar logs e corrigir configuração antes de anunciar a publicação como concluída.

## Verificação de aceite

1. Revisar/buildar a solução e gerar a imagem web em ambiente limpo.
2. Validar `docker compose config` sem exibir valores de segredos e verificar que só a aplicação e a proxy estão publicamente acessíveis.
3. Confirmar health checks, criação do esquema pelas migrations, e autenticação do administrador inicial; confirmar que as tabelas começam sem dados de negócio importados.
4. Validar DNS e acesso por `https://visosuplementosnaturais.com.br`, redirecionamento HTTP→HTTPS e operações web básicas.
5. Validar persistência após recriar o contêiner do banco e restaurar o backup em um banco de teste.

## Restrições e dependências

- A VPS identificada está ativa e tem recursos suficientes conforme inspeção anterior, mas o sistema operacional, estado atual do Docker/firewall e acesso SSH administrativo ainda precisam ser confirmados antes da implementação.
- Não há credenciais SSH Linux nem chave autorizada disponíveis nesta sessão. A senha VNC não equivale a credencial SSH. A etapa de implementação depende de acesso SSH seguro com privilégios administrativos; senhas ou chaves privadas não devem ser enviadas no chat.
- Acessibilidade pública também depende de DNS apontado corretamente e das portas 80/443 liberadas no firewall local e no firewall Contabo.
- A instalação do Caddy no host e a configuração de DNS são alterações externas necessárias ao deploy; serão feitas somente após o plano de implementação ser aprovado e o acesso seguro estar pronto.

