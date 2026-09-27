# Login por CPF com Usuario Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Replace Identity login with CPF/password authentication backed by `Usuario`, protected registration, and six-hour sessions.

**Architecture:** Keep the existing MVC account routes and cookie middleware, but replace Identity services with a focused user repository/authentication service. EF Core maps `Usuario`; ASP.NET Core `PasswordHasher<Usuario>` hashes passwords and claims are created from the authenticated user.

**Tech Stack:** .NET 10, ASP.NET Core MVC cookies, EF Core SQL Server, xUnit.

**Spec:** `docs/superpowers/specs/2026-09-27-login-usuario-design.md`

## Global Constraints

- CPF is normalized to digits only and unique.
- Passwords are never stored in plaintext.
- Authentication cookies expire absolutely after 6 hours.
- Registration requires `USER_REGISTRATION_KEY` from server configuration.
- Existing database tables are not deleted automatically.

## Review Focus

- CPF with punctuation/spaces: normalize consistently at registration and login.
- Duplicate CPF: reject without leaking whether an account exists.
- Inactive user: reject login with the same generic error as bad credentials.
- Expired six-hour cookie: redirect to `/Conta/Entrar`.
- Missing or incorrect registration key: deny registration without creating a user.

### Task 1: User model and EF mapping

**Files:** Create `VisoERP.Domain/Entities/Usuario.cs`; modify `VisoERP.Infra.Data/Context/VisoErpDbContext.cs`; create migration; test `VisoERP.Tests/UsuarioTests.cs`.

- [ ] Write failing tests for CPF normalization, required fields, unique CPF, and inactive state.
- [ ] Add `Usuario` entity and `DbSet<Usuario>` with unique index on normalized CPF.
- [ ] Update DbContext without inheriting `IdentityDbContext`; preserve existing entity mappings.
- [ ] Add EF migration that creates `Usuario` without dropping existing tables.
- [ ] Inspect generated migration and remove any destructive `AspNet*` drop operations; the existing Identity tables remain untouched but unused.
- [ ] Run focused tests and commit.

### Task 2: Authentication service and cookie claims

**Files:** Create `VisoERP.Infra.Auth/Services/UsuarioAuthService.cs`; modify auth DI/configuration; tests in `VisoERP.Tests/UsuarioAuthServiceTests.cs`.

- [ ] Test successful login, invalid password, inactive user, CPF normalization, and logout result.
- [ ] Implement `IUsuarioAuthService` using `PasswordHasher<Usuario>` and repository/DbContext.
- [ ] Replace Identity registration with cookie authentication and six-hour absolute expiration.
- [ ] Create claims for user id, name, and CPF; configure login path `/Conta/Entrar` and access denied behavior.
- [ ] Run tests and commit.

### Task 3: Login and protected registration MVC flow

**Files:** Modify `ContaController`, login view/model, and layout; create registration view/model; tests for controller/integration flow.

- [ ] Test login success redirect, generic failure, logout, six-hour cookie settings, and registration-key rejection.
- [ ] Update `/Conta/Entrar` to accept CPF and password through the new service.
- [ ] Add unlinked registration route requiring configured `USER_REGISTRATION_KEY`, with name/CPF/password/confirmation validation.
- [ ] Ensure protected controllers redirect unauthenticated requests to login.
- [ ] Remove Identity bootstrap and Identity-specific dependencies/usings.
- [ ] Run full non-LocalDB test suite and commit.

### Task 4: Deployment configuration and verification

**Files:** Modify `deploy/.env.example`, `deploy/README.md`, production Compose environment; add integration tests/documentation.

- [ ] Document `USER_REGISTRATION_KEY` as a server-only secret and the internal registration route.
- [ ] Update migrator/production environment so a first user can be created through the protected route.
- [ ] Build, run migrations against an empty database, and verify login, logout, expired-cookie redirect, and denied registration.
- [ ] Deploy updated containers to the VPS and verify `/health` plus the login page.
- [ ] Commit and record the deployment verification.
