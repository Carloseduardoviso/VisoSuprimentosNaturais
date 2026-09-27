# Restaurar um backup com segurança

Nunca substitua o banco em produção diretamente. Primeiro faça um backup atual e restaure o arquivo em um banco temporário para verificá-lo.

1. Entre na VPS por SSH e confirme o backup a testar, por exemplo `/srv/visoerp/backups/visoerp-AAAAMMDDTHHMMSSZ.bak`.
2. Gere um backup novo antes de qualquer restauração:

   ```bash
   sudo /opt/visoerp/current/deploy/backup.sh
   ```

3. Copie o arquivo escolhido para um nome de teste no mesmo diretório, por exemplo `restore-test.bak`. Não altere o backup original.
4. No diretório da versão atual, execute a restauração para o banco temporário `VisoERP_RestoreTest`. Ajuste somente o nome do arquivo:

   ```bash
   docker compose --env-file /opt/visoerp/shared/.env exec -T db \
     /bin/bash -lc '/opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -Q "$1"' -- \
     "RESTORE DATABASE [VisoERP_RestoreTest] FROM DISK = N'/var/opt/mssql/backup/restore-test.bak' WITH MOVE 'VisoERP' TO '/var/opt/mssql/data/VisoERP_RestoreTest.mdf', MOVE 'VisoERP_log' TO '/var/opt/mssql/data/VisoERP_RestoreTest_log.ldf', RECOVERY;"
   ```

   Antes de executar, use `RESTORE FILELISTONLY FROM DISK = ...` para confirmar os nomes lógicos (`VisoERP` e `VisoERP_log`). Troque-os no comando se o backup informar outros nomes.
5. Verifique as tabelas e dados no banco temporário. Não aponte o site para ele.
6. Remova somente o banco temporário e sua cópia de teste após a verificação:

   ```bash
   docker compose --env-file /opt/visoerp/shared/.env exec -T db \
     /bin/bash -lc '/opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -Q "$1"' -- \
     "ALTER DATABASE [VisoERP_RestoreTest] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [VisoERP_RestoreTest];"
   ```

Uma restauração sobre o banco `VisoERP` de produção só deve acontecer após parar o site, criar backup confirmado e ter um procedimento de janela de manutenção aprovado.
