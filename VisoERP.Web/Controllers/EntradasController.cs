using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisoERP.Application.DTOs.Estoque;
using VisoERP.Application.Interface.Cadastros;
using VisoERP.Application.Interface.Comercial;
using VisoERP.Application.Interface.Estoque;
using VisoERP.Web.Models.Estoque;

namespace VisoERP.Web.Controllers;

[Authorize(Policy = "GerenciarEstoque")]
public sealed class EntradasController(IEntradaEstoqueAppService entradas,
    IPedidoFornecedorAppService pedidos, ISuprimentoAppService suprimentos) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct) => View(await entradas.ListarAsync(ct));

    public async Task<IActionResult> Nova(Guid? pedidoFornecedorId, CancellationToken ct)
    {
        await PrepararOpcoes(ct);
        var model = new EntradaEstoqueViewModel { PedidoFornecedorId = pedidoFornecedorId };
        if (pedidoFornecedorId.HasValue)
        {
            var pedido = await pedidos.ObterAsync(pedidoFornecedorId.Value, ct);
            if (pedido is null) return NotFound();
            model.Itens = pedido.Itens.Where(x => x.QuantidadeRecebida < x.Quantidade)
                .Select(x => new ItemEntradaViewModel
                {
                    SuprimentoId = x.SuprimentoId, Quantidade = x.Quantidade - x.QuantidadeRecebida,
                    CustoUnitario = x.PrecoComDesconto
                }).ToList();
        }
        return View("Formulario", model);
    }

    [HttpPost]
    public async Task<IActionResult> Confirmar(EntradaEstoqueViewModel model, CancellationToken ct)
    {
        if (model.Itens.Count == 0) ModelState.AddModelError(string.Empty, "Adicione ao menos um suprimento.");
        if (ModelState.IsValid)
        {
            try
            {
                await entradas.ConfirmarAsync(new CriarEntradaDto(model.PedidoFornecedorId,
                    model.Itens.Select(x => new ItemEntradaDto(x.SuprimentoId!.Value, x.Quantidade,
                        x.CustoUnitario, new DateTimeOffset(x.Data), x.Promocional,
                        x.CodigoLote, x.Validade)).ToList(), model.VencimentoPagamento), ct);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or KeyNotFoundException)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }
        await PrepararOpcoes(ct);
        return View("Formulario", model);
    }

    private async Task PrepararOpcoes(CancellationToken ct)
    {
        var saldos = (await entradas.ListarSaldosAsync(ct)).ToDictionary(x => x.SuprimentoId);
        ViewBag.Suprimentos = (await suprimentos.ListarAsync(ct)).Where(x => x.Ativo)
            .Select(x => new { x.Id, x.Nome, Custo = saldos.TryGetValue(x.Id, out var s) ? s.CustoMedio : 0m })
            .ToList();
    }
}
