# VisoERP VPS Deployment Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Publish the VisoERP at `visosuplementosnaturais.com.br` with HTTPS, two persistent Docker Compose services (`web` and `db`), and a new empty SQL Server database.

**Architecture:** Build the ASP.NET Core application as a .NET 10 container and run SQL Server 2025 Express as a separate container on a private Compose network. Caddy runs as a host service, terminates HTTPS, and proxies only to the web service bound on loopback. A one-shot migration/setup tool initializes the schema and a least-privilege application login before the web service starts.

**Tech Stack:** .NET 10 / ASP.NET Core MVC, EF Core 10, SQL Server 2025 Express, Docker Engine + Compose, Caddy, Linux VPS, PowerShell/OpenSSH for Windows-side transfer.

**Spec:** `docs/superpowers/specs/2026-09-27-visoerp-vps-deploy-design.md`

## Global Constraints

- The application is ASP.NET Core MVC and targets `net10.0`.
- Keep exactly two long-running application containers named `web` and `db`; install Caddy as a host service.
- The SQL Server port 1433 must not be published on the VPS host or exposed publicly.
- The first production database starts empty; do not import LocalDB data.
- Never commit production secrets or expose them in command output/chat.
- The registered domain is `visosuplementosnaturais.com.br`; the paid term is one year.
- Keep the SQL data in a persistent volume and take a daily local backup; an off-VPS backup requires a later storage choice.
- Do not change VPS firewall rules until existing access is inspected and SSH access has been verified; never risk locking out SSH.

## Review Focus

- **Incorrect or untrusted proxy headers** can cause HTTPS redirect loops or spoofed request scheme; Task 6 tests direct HTTP, trusted forwarded HTTPS, and public Caddy HTTPS behavior.
- **Missing production configuration/secrets** can leave a partially running or insecure stack; Task 1 verifies Compose fails on required unset variables without printing secret values.
- **SQL starts slowly or fails** can cause the web process to fail bootstrap; Tasks 1 and 6 verify database health gating, restart behavior, and a successful web database health endpoint.
- **Database accidentally published to the host** can allow external access; Task 1 checks rendered Compose configuration has no `db.ports` and only binds the web origin to loopback.
- **Migration, backup, or restore failure** can lose operational data after launch; Tasks 3, 4, and 6 verify migrations on an empty database and restore a backup into a temporary test database.

---

### Task 1: Container packaging and host proxy configuration

**Files:**
- Create: `Dockerfile`
- Create: `.dockerignore`
- Create: `compose.yaml`
- Create: `deploy/Caddyfile`
- Create: `deploy/.env.example`

**Interfaces:**
- Produces the Compose services `web` and `db`, internal DNS name `db`, SQL data volume, and local-only web origin `127.0.0.1:8080` for Caddy.
- Required environment names: `MSSQL_SA_PASSWORD`, `APP_DB_PASSWORD`, `APP_CONNECTION_STRING`, `MIGRATION_CONNECTION_STRING`, `AUTOMAPPER_LICENSE_KEY`, and `FORWARDED_PROXY_IP`. `ADMIN_BOOTSTRAP_EMAIL` and `ADMIN_BOOTSTRAP_PASSWORD` are optional and empty after initial administrator creation.

- [ ] **Step 1: Define required Compose variables and assert missing configuration fails.** Use Compose required-variable syntax for runtime and migration secrets/domain/proxy values. Keep the two bootstrap administrator variables optional so later Compose updates still work after their removal. Validate with a deliberately incomplete temporary environment and confirm the error names only the missing key, never its value.
- [ ] **Step 2: Add a multi-stage .NET 10 Dockerfile.** Restore and publish `VisoERP.Web/VisoERP.Web.csproj` in Release mode; run the final ASP.NET image as the non-root `app` user on internal port 8080 and include `curl` for the Compose health check.
- [ ] **Step 3: Add `.dockerignore`.** Exclude `.git`, `bin`, `obj`, `.env*` while explicitly retaining `deploy/.env.example`, plus user-specific files, test output, and backups from the build context.
- [ ] **Step 4: Add `compose.yaml`.** Configure `web` and `db` on a private network; bind the web port only to `127.0.0.1:8080`; do not define `ports` for `db`; attach named volume `sql_data` to `/var/opt/mssql`; configure SQL Server 2025 Express and health check using the image's `sqlcmd`; gate web startup on `db` health.
- [ ] **Step 5: Add `deploy/Caddyfile`.** Redirect HTTP to HTTPS and reverse-proxy the canonical domain to `127.0.0.1:8080`; redirect `www.visosuplementosnaturais.com.br` to the canonical domain if DNS is configured for `www`.
- [ ] **Step 6: Add `deploy/.env.example`.** Include variable names, clearly fake placeholders for required fields, and empty optional bootstrap fields; never include a real password, key, account email, or secret.
- [ ] **Step 7: Validate Compose and exposure.** Run `docker compose --env-file <temporary-test-env> config --quiet`, inspect the rendered service port mappings without printing secrets, and assert `db` has no host-published port while `web` binds only loopback.

### Task 2: Reverse-proxy awareness and application health endpoint

**Files:**
- Modify: `VisoERP.Web/Program.cs`
- Create: `VisoERP.Web/Health/DatabaseHealthEndpoint.cs`
- Modify: `compose.yaml`

**Interfaces:**
- Consumes: `FORWARDED_PROXY_IP` from Compose, resolved to the actual Docker proxy source address during VPS setup.
- Produces: `GET /health`, returning `200` only when SQL Server is reachable and `503` otherwise, with no connection details in the response.
- Endpoint helper signature: `public static IEndpointRouteBuilder MapDatabaseHealthCheck(this IEndpointRouteBuilder endpoints)` in static class `DatabaseHealthEndpoint`.

- [ ] **Step 1: Add the database health endpoint implementation.** It calls `VisoErpDbContext.Database.CanConnectAsync()` and returns only a generic healthy/unavailable result; mark it anonymous so the fallback authorization policy does not block container health checks.
- [ ] **Step 2: Configure forwarded headers from the single trusted proxy IP.** Parse `FORWARDED_PROXY_IP` as an IP address, configure `X-Forwarded-For` and `X-Forwarded-Proto` with a one-proxy limit, and fail production startup when the value is absent/invalid. Do not trust all proxies/networks.
- [ ] **Step 3: Order middleware.** Call forwarded-header middleware before HSTS and `UseHttpsRedirection`; map `/health` without weakening authentication for other routes.
- [ ] **Step 4: Add the web Compose health check.** Probe `http://127.0.0.1:8080/health` with a bounded timeout and retries; this reports unhealthy until the application can connect to SQL Server.
- [ ] **Step 5: Build and run app tests.** Run `dotnet build VisoERP.slnx -c Release` and `dotnet test VisoERP.slnx --filter "FullyQualifiedName!~PedidoEntradaIntegracaoTests"`; expected: build succeeds and all non-LocalDB tests pass.
### Task 3: Explicit database migration and least-privilege application login

**Files:**
- Create: `VisoERP.DatabaseMigrator/VisoERP.DatabaseMigrator.csproj`
- Create: `VisoERP.DatabaseMigrator/Program.cs`
- Create: `VisoERP.DatabaseMigrator/DatabasePrincipalProvisioner.cs`
- Create: `deploy/migrator.Dockerfile`
- Create: `VisoERP.Tests/DatabasePrincipalProvisionerTests.cs`
- Modify: `VisoERP.slnx`
- Modify: `VisoERP.Tests/VisoERP.Tests.csproj`
- Modify: `compose.yaml`
- Modify: `deploy/.env.example`

**Interfaces:**
- The one-shot `migrate` profile receives an administrative SQL connection string and an app login/password, runs EF migrations, creates a dedicated SQL login/user with only `db_datareader` and `db_datawriter`, then exits nonzero on any failure.
- The long-running `web` service receives only `APP_CONNECTION_STRING`; it never receives the SQL `sa` password.
- Helper signatures: `DatabasePrincipalProvisioner.QuoteIdentifier(string value) -> string` (returns bracket-quoted identifier), `QuoteLiteral(string value) -> string` (returns an `N'...'` SQL literal), and `ProvisionAsync(SqlConnection openConnection, string databaseName, string loginName, string password, CancellationToken cancellationToken) -> Task`.

- [ ] **Step 1: Add `DatabasePrincipalProvisioner`.** Implement the listed signatures; provision a fixed-format login/database user and grant only reader/writer roles; quote SQL identifiers and escape SQL string literals rather than concatenating unchecked user input.
- [ ] **Step 2: Add the migrator project.** Reference `VisoERP.Infra.Data`; read the administrative connection and app credential from environment; call `Database.MigrateAsync()` against `VisoErpDbContext`; create the restricted app principal only after migrations succeed; return a nonzero exit code and no secret text on failure.
- [ ] **Step 3: Add the migrator container target/service.** Build from the .NET 10 SDK image and run only on demand under Compose profile `ops`; it must not be a third long-running container.
- [ ] **Step 4: Configure Compose connection separation.** The `migrate` job uses `MIGRATION_CONNECTION_STRING`; `web` uses `APP_CONNECTION_STRING`. Never pass the administrative SQL credential to `web`.
- [ ] **Step 5: Add focused tests in `VisoERP.Tests/DatabasePrincipalProvisionerTests.cs`.** Verify SQL principal quoting escapes an apostrophe/closing bracket safely and rejects empty or invalid credentials; verify missing administrative configuration fails before attempting a connection.
- [ ] **Step 6: Add the migrator to `VisoERP.slnx` and verify.** Run `dotnet build VisoERP.slnx -c Release` and `dotnet test VisoERP.slnx --filter "FullyQualifiedName!~PedidoEntradaIntegracaoTests"`; expected: all pass.
- [ ] **Step 7: Verify the migration job on the VPS SQL container.** Run migrations against a newly created empty database; verify all migrations are applied, the app login cannot perform DDL, and the web identity bootstrap can create roles/admin using the app login.

### Task 4: Operational runbook, secrets, backups, and restore

**Files:**
- Create: `deploy/README.md`
- Create: `deploy/backup.sh`
- Create: `deploy/restore.md`
- Modify: `compose.yaml`

**Interfaces:**
- Backup script reads the database secret from the protected SQL container environment, writes timestamped `.bak` files to `/srv/visoerp/backups`, and never prints credentials.
- Retention: keep the latest 7 daily backups and 4 weekly backups on the VPS.

- [ ] **Step 1: Configure persistent backup storage.** Add a host bind mount for SQL Server backups separate from the database data volume; document owner/permissions for the SQL Server container user.
- [ ] **Step 2: Implement `deploy/backup.sh`.** Use the SQL Server 2025 image's `/opt/mssql-tools18/bin/sqlcmd` to create a checksum-protected backup; enforce strict shell error handling, validate non-empty output, and apply 7-daily/4-weekly retention without deleting the newest valid backup.
- [ ] **Step 3: Document restore to a temporary database.** Provide an explicit, non-destructive restore procedure and prohibit overwriting the live database without a verified pre-restore backup.
- [ ] **Step 4: Write `deploy/README.md`.** Document secret creation on the VPS with mode `0600`, one-time admin bootstrap/removal, DNS, Caddy, Compose start/update/log commands, migrations, backups, restore, and safe rollback. State that off-VPS copies require a separately approved destination.
- [ ] **Step 5: Test backup/retention/restore.** On the VPS, verify a dated `.bak` is non-empty, restore it to a temporary database, confirm tables/data, then remove only that temporary test database and test backup artifact.

### Task 5: Prepare VPS access, DNS, firewall, Docker, and Caddy

**Files:**
- Use: `deploy/Caddyfile`
- Follow: `deploy/README.md`

**Interfaces:**
- Requires verified SSH key-based access with administrative privileges; private keys/passwords must not be sent in chat.
- Host target: existing Contabo VPS; domain: `visosuplementosnaturais.com.br`.

- [ ] **Step 1: Establish secure SSH access.** Verify the SSH host fingerprint and administrative login before any remote mutation. If no authorized key is available, pause until the owner installs a public key through the trusted VPS console.
- [ ] **Step 2: Inspect before changing the host.** Record OS/version, disk/memory, current Docker/Caddy state, running services on 80/443, firewall rules, and whether `/opt/visoerp` or `/srv/visoerp` already exists; preserve existing data/services.
- [ ] **Step 3: Install Docker Engine and Compose plugin if absent.** Use the official package source for the detected Linux distribution; verify `docker version` and `docker compose version`.
- [ ] **Step 4: Install Caddy as a host service if absent.** Do not overwrite an existing Caddy configuration without backing it up; validate the proposed Caddyfile before reload.
- [ ] **Step 5: Configure firewall safely.** Ensure the currently used SSH path remains allowed before changes; allow public TCP 80/443 only, keep 1433 closed, and verify SSH in a second session before closing the first.
- [ ] **Step 6: Configure DNS.** Point the apex A record and optional `www` A record to the Contabo IPv4; confirm DNS resolves to the VPS before requesting certificates. Do not change nameservers unless required by inspected current configuration.
- [ ] **Step 7: Verify readiness.** Confirm HTTP/HTTPS ports reach the VPS, Caddy config validates, Docker is enabled at boot, and no pre-existing service was displaced.

### Task 6: Deploy, initialize the blank database, and verify production behavior

**Files:**
- Use: `Dockerfile`, `compose.yaml`, `deploy/Caddyfile`, `deploy/README.md`, and the VPS `/opt/visoerp` release directory.

**Interfaces:**
- Consumes verified SSH access, DNS, Caddy, Docker, and protected environment file from Tasks 1–5.
- Produces two running containers (`web`, `db`), an empty-schema database with admin account only, and public HTTPS at the registered domain.

- [ ] **Step 1: Transfer a clean source release.** Create an archive from the reviewed Git commit, excluding `.git`, ignored local secrets, build output, and LocalDB files; transfer to `/opt/visoerp/releases/<commit>` without overwriting prior releases.
- [ ] **Step 2: Create the protected server environment file.** Copy the example and have the owner enter the AutoMapper key, admin email, and chosen strong password directly in the trusted server session; generate SQL passwords locally/on-host and set mode `0600`. Do not echo or record values.
- [ ] **Step 3: Validate and start only the database.** Run Compose config validation without rendering secrets, start `db`, and wait for its health check before proceeding.
- [ ] **Step 4: Apply migrations and create the web login.** Run the one-shot `migrate` service and stop on any nonzero result; verify every expected migration is present and the database contains no imported business rows.
- [ ] **Step 5: Start the web service and initialize identity.** Start `web`; verify `/health` is healthy, the four built-in roles exist, and the bootstrap administrator can sign in. Remove bootstrap email/password from the server environment after first successful creation and restart `web`.
- [ ] **Step 6: Activate and smoke-test Caddy and trusted forwarding.** Validate and reload Caddy; test apex HTTPS, HTTP-to-HTTPS redirect, a trusted forwarded HTTPS request with no redirect loop, a direct HTTP request without trusted forwarding, login page, authentication, static assets, and basic read-only pages. Confirm `db` has no public IP/port and the domain serves no development-only anonymous access.
- [ ] **Step 7: Install and run daily backup.** Schedule `deploy/backup.sh` at 02:00 server-local time, perform one backup/restore verification, and record the backup location/retention in the handoff.
- [ ] **Step 8: Verify restart and rollback.** Recreate the web container and confirm data persists; confirm previous app release can be restored without deleting the DB volume; do not claim deployment complete until all acceptance checks pass.
