# Operação de produção do VisoERP

## Estrutura na VPS

Use diretórios separados para versões, configuração e backups:

```text
/opt/visoerp/releases/<commit>
/opt/visoerp/current -> /opt/visoerp/releases/<commit>
/opt/visoerp/shared/.env
/srv/visoerp/backups
```

O Compose mantém apenas dois serviços permanentes: `web` e `db`. O serviço `migrate` usa o perfil `ops`, executa uma vez para aplicar migrations/criar a conta da aplicação e termina. O Caddy é instalado como serviço do host, fora do Compose.

## Segredos e primeiro administrador

Na VPS, copie o exemplo e limite o acesso:

```bash
sudo install -d -m 0700 /opt/visoerp/shared /srv/visoerp/backups
sudo chown 10001:0 /srv/visoerp/backups
sudo cp /opt/visoerp/current/deploy/.env.example /opt/visoerp/shared/.env
sudo chmod 0600 /opt/visoerp/shared/.env
sudo chown root:root /opt/visoerp/shared/.env
sudoedit /opt/visoerp/shared/.env
```

Preencha senhas SQL fortes, a licença AutoMapper e o e-mail/senha forte do primeiro administrador. As connection strings devem referenciar `db,1433`, usar o banco `VisoERP`, e ter `Encrypt=True;TrustServerCertificate=True`.

Após o primeiro login administrativo funcionar, remova os valores de `ADMIN_BOOTSTRAP_EMAIL` e `ADMIN_BOOTSTRAP_PASSWORD` do arquivo e reinicie o site. Nunca inclua esse arquivo no Git, em um backup público ou em mensagens.

## Subida e atualização

No diretório apontado por `/opt/visoerp/current`:

```bash
docker compose --env-file /opt/visoerp/shared/.env config --quiet
docker compose --env-file /opt/visoerp/shared/.env up -d db
docker compose --env-file /opt/visoerp/shared/.env ps
docker compose --env-file /opt/visoerp/shared/.env --profile ops run --rm migrate
docker compose --env-file /opt/visoerp/shared/.env up -d web
docker compose --env-file /opt/visoerp/shared/.env ps
```

Para uma atualização, crie um backup verificado antes de trocar o link `current`, valide a nova versão com `docker compose config --quiet`, execute o migrador e só então suba o `web`. Para voltar a uma versão anterior do site, retorne o link `current` e recrie apenas o contêiner `web`; não remova o volume `sql_data`. Alterações de schema só voltam com restauração de backup.

## Caddy, DNS e HTTPS

Copie `deploy/Caddyfile` para `/etc/caddy/Caddyfile`. Defina `CADDY_EMAIL` em uma configuração systemd do Caddy (não no arquivo do Compose), valide e recarregue:

```bash
sudo caddy validate --config /etc/caddy/Caddyfile
sudo systemctl reload caddy
```

No Registro.br, aponte os registros A de `visosuplementosnaturais.com.br` e `www.visosuplementosnaturais.com.br` para o IPv4 da VPS. Só peça certificados após os dois nomes resolverem para a VPS. O firewall público expõe somente TCP 80/443 e SSH administrativo; não publique a porta 1433.

## Backup diário e restauração

Instale o script como executável e agende-o para 02:00 no horário da VPS:

```bash
sudo chmod 0750 /opt/visoerp/current/deploy/backup.sh
sudo crontab -e
# 0 2 * * * /opt/visoerp/current/deploy/backup.sh >> /var/log/visoerp-backup.log 2>&1
```

O script cria backup com checksum e `RESTORE VERIFYONLY`, guarda os sete backups diários mais recentes e quatro semanais. Faça uma restauração em banco temporário conforme [restore.md](restore.md) antes de depender do processo. Esses backups ficam na própria VPS; uma cópia externa exige escolher e aprovar o destino de armazenamento.

## Diagnóstico e verificações obrigatórias

```bash
docker compose --env-file /opt/visoerp/shared/.env logs --tail=200 web
docker compose --env-file /opt/visoerp/shared/.env logs --tail=200 db
curl --fail http://127.0.0.1:8080/health
curl --fail --location https://visosuplementosnaturais.com.br/health
```

O endpoint de saúde retorna `healthy` apenas quando o SQL Server está acessível. Antes de anunciar a publicação, confirme HTTPS, redirecionamento HTTP→HTTPS, tela de login, acesso do administrador, arquivos estáticos, banco sem porta pública, migrations aplicadas e persistência após recriar o contêiner web.
