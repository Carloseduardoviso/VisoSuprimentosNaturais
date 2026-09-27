#!/usr/bin/env bash
set -euo pipefail

readonly DEPLOY_DIRECTORY="${DEPLOY_DIRECTORY:-/opt/visoerp/current}"
readonly ENV_FILE="${ENV_FILE:-/opt/visoerp/shared/.env}"
readonly BACKUP_DIRECTORY="${BACKUP_DIRECTORY:-/srv/visoerp/backups}"
readonly DATABASE_NAME="${DATABASE_NAME:-VisoERP}"

if [[ ! -r "$ENV_FILE" ]]; then
  echo "Protected environment file is not readable." >&2
  exit 1
fi

mkdir -p "$BACKUP_DIRECTORY"

readonly timestamp="$(date -u +%Y%m%dT%H%M%SZ)"
readonly backup_name="visoerp-${timestamp}.bak"
readonly container_backup_path="/var/opt/mssql/backup/${backup_name}"
readonly host_backup_path="${BACKUP_DIRECTORY}/${backup_name}"

cd "$DEPLOY_DIRECTORY"
readonly backup_query="BACKUP DATABASE [${DATABASE_NAME}] TO DISK = N'${container_backup_path}' WITH CHECKSUM, INIT, STATS = 10; RESTORE VERIFYONLY FROM DISK = N'${container_backup_path}' WITH CHECKSUM;"
docker compose --env-file "$ENV_FILE" exec -T db \
  /bin/bash -lc '/opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -Q "$1"' \
  -- "$backup_query"

if [[ ! -s "$host_backup_path" ]]; then
  echo "Backup file was not created or is empty." >&2
  exit 1
fi

if [[ "$(date -u +%u)" == "7" ]]; then
  cp -- "$host_backup_path" "${BACKUP_DIRECTORY}/weekly-${timestamp}.bak"
fi

mapfile -t daily_backups < <(find "$BACKUP_DIRECTORY" -maxdepth 1 -type f -name 'visoerp-*.bak' -printf '%T@ %p\n' | sort -nr | cut -d' ' -f2-)
for ((index = 7; index < ${#daily_backups[@]}; index++)); do
  rm -f -- "${daily_backups[$index]}"
done

mapfile -t weekly_backups < <(find "$BACKUP_DIRECTORY" -maxdepth 1 -type f -name 'weekly-*.bak' -printf '%T@ %p\n' | sort -nr | cut -d' ' -f2-)
for ((index = 4; index < ${#weekly_backups[@]}; index++)); do
  rm -f -- "${weekly_backups[$index]}"
done

echo "Backup verified: ${backup_name}"
