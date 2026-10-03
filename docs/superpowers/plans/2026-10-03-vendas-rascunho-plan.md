# Vendas em Rascunho Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Permitir salvar, editar, excluir e finalizar vendas em rascunho sem efeitos reais antes da finalização.

**Architecture:** Separar persistência do rascunho da efetivação da venda. O rascunho salva a venda e seus itens sem estoque/financeiro; a finalização reaproveita a validação atual e executa estoque, parcelas e recebimentos em uma transação. A UI terá endpoints e ações explícitos por estado.

**Tech Stack:** .NET 10, ASP.NET Core MVC, EF Core SQL Server, xUnit.

**Spec:** `docs/superpowers/specs/2026-10-03-vendas-rascunho-design.md`

## Global Constraints

- O cliente é obrigatório para salvar um rascunho.
- Rascunhos não baixam estoque, não criam recebimentos e não entram nos indicadores.
- Apenas a finalização torna a venda real.
- Vendas finalizadas não podem ser editadas ou excluídas pelo fluxo de rascunho.
- Não remover a migration `20261003171348_RepararFinalizadaVenda`.

## Review Focus

- Finalização com estoque insuficiente deve manter rascunho e estoque intactos — teste no app service.
- Edição deve substituir itens sem duplicá-los — teste no app service/repositório.
- Exclusão deve remover dependências do rascunho — teste de integração do contexto.
- Venda finalizada não pode passar por endpoints de rascunho — teste do domínio/controller.
- Rascunho não pode aparecer nos indicadores financeiros — teste do serviço financeiro.

### Task 1: Domain and application contracts

**Files:**
- Modify: `VisoERP.Application/DTOs/Comercial/VendaDto.cs`
- Modify: `VisoERP.Application/Interface/Comercial/IVendaAppService.cs`
- Modify: `VisoERP.Domain/Entities/Comercial/Venda.cs`
- Test: `VisoERP.Tests/VendaTests.cs`

- [ ] Write failing tests for updating a draft, finalizing only once, and rejecting mutation after finalization.
- [ ] Run `dotnet test VisoERP.Tests/VisoERP.Tests.csproj --filter FullyQualifiedName~VendaTests` and confirm the new tests fail for missing API/behavior.
- [ ] Add draft-aware DTO state and domain methods needed to replace editable fields/items while preserving `Finalizada`.
- [ ] Run the focused tests and confirm pass.
- [ ] Run the existing domain tests.

### Task 2: Persistence and app service draft lifecycle

**Files:**
- Modify: `VisoERP.Domain/Interfaces/Repositories/Comercial/IVendaRepository.cs`
- Modify: `VisoERP.Infra.Data/Repositories/Comercial/VendaRepository.cs`
- Modify: `VisoERP.Application/AppService/Comercial/VendaAppService.cs`
- Modify: `VisoERP.Application/AutoMapper/Comercial/VendaProfile.cs`
- Test: `VisoERP.Tests/VendaDraftTests.cs`

- [ ] Write failing integration/service tests for save draft without stock movement, update draft without duplication, delete draft, successful finalization, and rollback on insufficient stock.
- [ ] Run the focused tests and verify failure is caused by the missing draft lifecycle.
- [ ] Implement `SalvarRascunhoAsync`, `AtualizarRascunhoAsync`, `FinalizarAsync`, and `ExcluirRascunhoAsync`; move stock/receipt/parcel creation exclusively into finalization.
- [ ] Add repository support for draft loading and deletion; enforce `Finalizada == false` in draft operations.
- [ ] Run focused tests and full `dotnet test VisoERP.Tests/VisoERP.Tests.csproj`.

### Task 3: MVC workflow and views

**Files:**
- Modify: `VisoERP.Web/Controllers/VendasController.cs`
- Modify: `VisoERP.Web/Models/Comercial/VendaViewModel.cs`
- Modify: `VisoERP.Web/Views/Vendas/Formulario.cshtml`
- Modify: `VisoERP.Web/Views/Vendas/Index.cshtml`
- Modify: `VisoERP.Web/Views/Vendas/Detalhes.cshtml`
- Test: `VisoERP.Web.Tests`

- [ ] Add failing controller/view tests or focused source assertions for draft status, edit action, save/back, finalize, and delete controls.
- [ ] Run the focused web tests and confirm failure.
- [ ] Add routes/actions for new draft, edit draft, save draft, finalize, and delete draft with client validation.
- [ ] Load existing draft values/items into the form and show status/action buttons conditionally.
- [ ] Run web tests and the full solution test suite.

### Task 4: Financial filtering and verification

**Files:**
- Modify: `VisoERP.Application/AppService/Financeiro/FinanceiroAppService.cs`
- Modify: relevant finance tests
- Modify: `VisoERP.Web.Tests` if needed

- [ ] Add a failing test proving drafts do not contribute to revenue, received totals, or gross result.
- [ ] Filter sales/receipts used by finance to finalized sales only, preserving existing finalized behavior.
- [ ] Run finance tests and full `dotnet test`.
- [ ] Build the solution and manually verify the migration/database remains compatible.

### Task 5: Final verification

- [ ] Review `git diff` for state guards, transaction boundaries, and accidental unrelated changes.
- [ ] Run `dotnet test` for the full solution.
- [ ] Run `dotnet build VisoERP.slnx --no-restore`.
- [ ] Verify the local database migration state with `dotnet ef database update`.
