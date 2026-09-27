using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisoERP.Application.Interface.Financeiro;
using VisoERP.Web.Models.Financeiro;

namespace VisoERP.Web.Controllers;

[Authorize(Policy = "GerenciarFinanceiro")]
public sealed class FinanceiroController(IFinanceiroAppService financeiro) : Controller
{
    public async Task<IActionResult> Index(DateOnly? inicio, DateOnly? fim, CancellationToken ct)
    {
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var dataInicio = inicio ?? new DateOnly(hoje.Year, hoje.Month, 1);
        var dataFim = fim ?? hoje;
        if (dataInicio > dataFim) return BadRequest("Período inválido.");
        return View(new FinanceiroViewModel
        {
            Inicio = dataInicio, Fim = dataFim,
            Resumo = await financeiro.ResumirAsync(dataInicio, dataFim, ct),
            Investimentos = await financeiro.ListarInvestimentosAsync(ct),
            Despesas = await financeiro.ListarDespesasAsync(ct),
            ContasPagar = await financeiro.ListarContasPagarAsync(ct)
        });
    }

    [HttpPost]
    public async Task<IActionResult> Investir(RegistroFinanceiroViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) TempData["Erro"] = "Confira os dados do investimento.";
        else
        {
            try { await financeiro.RegistrarInvestimentoAsync(model.Descricao, model.Valor, model.Data, ct); }
            catch (ArgumentException ex) { TempData["Erro"] = ex.Message; }
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Despesa(RegistroFinanceiroViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) TempData["Erro"] = "Confira os dados da despesa.";
        else
        {
            try { await financeiro.RegistrarDespesaAsync(model.Descricao, model.Valor, model.Data, model.Paga, ct); }
            catch (ArgumentException ex) { TempData["Erro"] = ex.Message; }
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> PagarDespesa(Guid id, CancellationToken ct)
    {
        try { await financeiro.PagarDespesaAsync(id, ct); }
        catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException)
        { TempData["Erro"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> PagarConta(Guid id, decimal valor, CancellationToken ct)
    {
        try { await financeiro.PagarContaAsync(id, valor, ct); }
        catch (Exception ex) when (ex is ArgumentException or KeyNotFoundException)
        { TempData["Erro"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }
}
