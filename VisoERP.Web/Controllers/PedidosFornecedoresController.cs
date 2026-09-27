using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VisoERP.Application.DTOs.Comercial;
using VisoERP.Application.Interface.Cadastros;
using VisoERP.Application.Interface.Comercial;
using VisoERP.Web.Models.Comercial;

namespace VisoERP.Web.Controllers;

[Authorize(Policy = "GerenciarEstoque")]
public sealed class PedidosFornecedoresController(IPedidoFornecedorAppService pedidos,
    IFornecedorAppService fornecedores, ISuprimentoAppService suprimentos) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct) => View(await pedidos.ListarAsync(ct));

    public async Task<IActionResult> Novo(CancellationToken ct)
    {
        await PrepararOpcoes(ct);
        return View("Formulario", new PedidoFornecedorViewModel());
    }

    public async Task<IActionResult> Detalhes(Guid id, CancellationToken ct)
    {
        var pedido = await pedidos.ObterAsync(id, ct);
        return pedido is null ? NotFound() : View(pedido);
    }

    [HttpPost]
    public async Task<IActionResult> Salvar(PedidoFornecedorViewModel model, CancellationToken ct)
    {
        if (model.Itens.Count == 0) ModelState.AddModelError(string.Empty, "Adicione ao menos um suprimento.");
        if (ModelState.IsValid)
        {
            try
            {
                var id = await pedidos.CriarAsync(new CriarPedidoDto(model.FornecedorId!.Value,
                    model.Itens.Select(x => new CriarItemPedidoDto(x.SuprimentoId!.Value, x.Quantidade,
                        x.PrecoCatalogo, x.PrecoComDesconto)).ToList()), ct);
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
    public async Task<IActionResult> Cancelar(Guid id, CancellationToken ct)
    {
        try { await pedidos.CancelarAsync(id, ct); }
        catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException)
        {
            TempData["Erro"] = ex.Message;
        }
        return RedirectToAction(nameof(Detalhes), new { id });
    }

    private async Task PrepararOpcoes(CancellationToken ct)
    {
        ViewBag.Fornecedores = (await fornecedores.ListarAsync(ct))
            .Select(x => new SelectListItem(x.Nome, x.Id.ToString())).ToList();
        ViewBag.Suprimentos = (await suprimentos.ListarAsync(ct)).Where(x => x.Ativo)
            .Select(x => new { x.Id, x.Nome, x.PrecoCatalogo, x.PrecoComDesconto }).ToList();
    }
}
