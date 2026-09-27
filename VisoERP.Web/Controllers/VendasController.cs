using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VisoERP.Application.DTOs.Comercial;
using VisoERP.Application.Interface.Cadastros;
using VisoERP.Application.Interface.Comercial;
using VisoERP.Application.Interface.Estoque;
using VisoERP.Web.Models.Comercial;

namespace VisoERP.Web.Controllers;

[Authorize(Policy = "GerenciarVendas")]
public sealed class VendasController(IVendaAppService vendas, IClienteAppService clientes,
    ISuprimentoAppService suprimentos, IEntradaEstoqueAppService entradas) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        ViewBag.Clientes = (await clientes.ListarAsync(ct)).ToDictionary(x => x.Id, x => x.Nome);
        return View(await vendas.ListarAsync(ct));
    }

    public async Task<IActionResult> Nova(CancellationToken ct)
    {
        await PrepararOpcoes(ct);
        return View("Formulario", new VendaViewModel());
    }

    public async Task<IActionResult> Detalhes(Guid id, CancellationToken ct)
    {
        var venda = await vendas.ObterAsync(id, ct);
        if (venda is null) return NotFound();
        ViewBag.Cliente = (await clientes.ObterAsync(venda.ClienteId, ct))?.Nome ?? venda.ClienteId.ToString();
        return View(venda);
    }

    [HttpPost]
    public async Task<IActionResult> Registrar(VendaViewModel model, CancellationToken ct)
    {
        if (model.Itens.Count == 0) ModelState.AddModelError(string.Empty, "Adicione ao menos um suplemento alimentar.");
        if (ModelState.IsValid)
        {
            try
            {
                var id = await vendas.RegistrarAsync(new CriarVendaDto(model.ClienteId!.Value,
                    model.Desconto, model.ValorEntrada, model.NumeroParcelas, model.PrimeiroVencimento,
                    model.Itens.Select(x => new CriarItemVendaDto(x.SuprimentoId!.Value, x.Quantidade,
                        x.PrecoCatalogo, x.PrecoUnitario, x.Promocional,
                        new DateTimeOffset(x.Data))).ToList()), ct);
                return RedirectToAction(nameof(Detalhes), new { id });
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or KeyNotFoundException)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }
        await PrepararOpcoes(ct);
        return View("Formulario", model);
    }

    [HttpPost]
    public async Task<IActionResult> Pagar(Guid id, Guid parcelaId, decimal valor, CancellationToken ct)
    {
        try { await vendas.RegistrarPagamentoAsync(id, parcelaId, valor, DateTimeOffset.UtcNow, ct); }
        catch (Exception ex) when (ex is ArgumentException or KeyNotFoundException)
        {
            TempData["Erro"] = ex.Message;
        }
        return RedirectToAction(nameof(Detalhes), new { id });
    }

    private async Task PrepararOpcoes(CancellationToken ct)
    {
        var saldos = (await entradas.ListarSaldosAsync(ct))
            .ToDictionary(x => x.SuprimentoId, x => x.Quantidade);
        ViewBag.Clientes = (await clientes.ListarAsync(ct)).Where(x => x.Ativo)
            .Select(x => new SelectListItem(x.Nome, x.Id.ToString())).ToList();
        ViewBag.Suprimentos = (await suprimentos.ListarAsync(ct)).Where(x => x.Ativo)
            .Select(x => new { x.Id, x.Nome, x.PrecoCatalogo, x.PrecoComDesconto,
                Estoque = saldos.GetValueOrDefault(x.Id),
            }).ToList();
    }
}
