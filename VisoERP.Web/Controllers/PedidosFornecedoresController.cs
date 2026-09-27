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
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        ViewBag.Fornecedores = (await fornecedores.ListarAsync(ct))
            .ToDictionary(x => x.Id, x => x.Nome);
        return View(await pedidos.ListarAsync(ct));
    }

    public async Task<IActionResult> Novo(CancellationToken ct)
    {
        await PrepararOpcoes(ct);
        return View("Formulario", new PedidoFornecedorViewModel());
    }

    public async Task<IActionResult> Copiar(Guid id, CancellationToken ct)
    {
        var pedido = await pedidos.ObterAsync(id, ct);
        if (pedido is null) return NotFound();
        await PrepararOpcoes(ct);
        return View("Formulario", new PedidoFornecedorViewModel
        {
            FornecedorId = pedido.FornecedorId,
            Itens = pedido.Itens.Select(x => new ItemPedidoViewModel
            {
                SuprimentoId = x.SuprimentoId, Quantidade = x.Quantidade,
                PrecoCatalogo = x.PrecoCatalogo, PrecoComDesconto = x.PrecoComDesconto
            }).ToList()
        });
    }

    public async Task<IActionResult> WhatsApp(Guid id, CancellationToken ct)
    {
        var pedido = await pedidos.ObterAsync(id, ct);
        if (pedido is null) return NotFound();
        var fornecedor = await fornecedores.ObterAsync(pedido.FornecedorId, ct);
        var telefone = fornecedor?.Telefone?.Where(char.IsDigit).ToArray() ?? [];
        if (telefone.Length is < 10 or > 13) return RedirectToAction(nameof(Detalhes), new { id });
        var numero = new string(telefone);
        if (numero.Length is 10 or 11) numero = "55" + numero;
        var nomes = (await suprimentos.ListarAsync(ct)).ToDictionary(x => x.Id, x => x.Nome);
        var cultura = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
        var linhas = pedido.Itens.Select(item =>
        {
            var nome = nomes.TryGetValue(item.SuprimentoId, out var suplemento) ? suplemento : "Suplemento alimentar";
            return $"• {nome}\n  Quantidade: {item.Quantidade.ToString("0", cultura)}\n  Preço catálogo: {item.PrecoCatalogo.ToString("C", cultura)}\n  Preço com desconto: {item.PrecoComDesconto.ToString("C", cultura)}\n  Total: {item.Total.ToString("C", cultura)}";
        });
        var texto = $"Olá, {fornecedor?.Nome ?? "fornecedor"}!\n\nGostaria de fazer este pedido:\n\n{string.Join("\n\n", linhas)}\n\nTotal do pedido: {pedido.Total.ToString("C", cultura)}\n\nAguardo sua confirmação. Obrigado!";
        var mensagem = Uri.EscapeDataString(texto);
        return Redirect($"https://wa.me/{numero}?text={mensagem}");
    }

    public async Task<IActionResult> Mensagem(Guid id, CancellationToken ct)
    {
        var pedido = await pedidos.ObterAsync(id, ct);
        if (pedido is null) return NotFound();
        var fornecedor = await fornecedores.ObterAsync(pedido.FornecedorId, ct);
        var nomes = (await suprimentos.ListarAsync(ct)).ToDictionary(x => x.Id, x => x.Nome);
        var cultura = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
        var linhas = pedido.Itens.Select(item =>
        {
            var nome = nomes.TryGetValue(item.SuprimentoId, out var suplemento) ? suplemento : "Suplemento alimentar";
            return $"• {nome}\n  Quantidade: {item.Quantidade.ToString("0", cultura)}\n  Preço catálogo: {item.PrecoCatalogo.ToString("C", cultura)}\n  Preço com desconto: {item.PrecoComDesconto.ToString("C", cultura)}\n  Total: {item.Total.ToString("C", cultura)}";
        });
        var mensagem = $"Olá, {fornecedor?.Nome ?? "fornecedor"}!\n\nGostaria de fazer este pedido:\n\n{string.Join("\n\n", linhas)}\n\nTotal do pedido: {pedido.Total.ToString("C", cultura)}\n\nAguardo sua confirmação. Obrigado!";
        return Content(mensagem, "text/plain; charset=utf-8");
    }

    public async Task<IActionResult> Detalhes(Guid id, CancellationToken ct)
    {
        var pedido = await pedidos.ObterAsync(id, ct);
        if (pedido is null) return NotFound();
        ViewBag.Suplementos = (await suprimentos.ListarAsync(ct))
            .ToDictionary(x => x.Id, x => x.Nome);
        return View(pedido);
    }

    [HttpPost]
    public async Task<IActionResult> Salvar(PedidoFornecedorViewModel model, CancellationToken ct)
    {
        if (model.Itens.Count == 0) ModelState.AddModelError(string.Empty, "Adicione ao menos um suplemento alimentar.");
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
        try
        {
            await pedidos.CancelarAsync(id, ct);
            var pedido = await pedidos.ObterAsync(id, ct);
            var fornecedor = pedido is null ? null : await fornecedores.ObterAsync(pedido.FornecedorId, ct);
            var telefone = fornecedor?.Telefone?.Where(char.IsDigit).ToArray() ?? [];
            if (telefone.Length is 10 or 11)
            {
                var numero = "55" + new string(telefone);
                var cultura = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
                var nomes = (await suprimentos.ListarAsync(ct)).ToDictionary(x => x.Id, x => x.Nome);
                var itens = pedido!.Itens.Select(item =>
                {
                    var nome = nomes.TryGetValue(item.SuprimentoId, out var suplemento) ? suplemento : "Suplemento alimentar";
                    return $"• {nome}\n  Quantidade: {item.Quantidade.ToString("0", cultura)}\n  Preço catálogo: {item.PrecoCatalogo.ToString("C", cultura)}\n  Preço com desconto: {item.PrecoComDesconto.ToString("C", cultura)}\n  Total: {item.Total.ToString("C", cultura)}";
                });
                var texto = $"Olá, {fornecedor!.Nome}!\n\nInformamos que o pedido abaixo foi cancelado:\n\n{string.Join("\n\n", itens)}\n\nTotal do pedido cancelado: {pedido.Total.ToString("C", cultura)}\n\nPedimos desculpas pelo transtorno e agradecemos a compreensão.";
                var mensagem = Uri.EscapeDataString(texto);
                return Redirect($"https://wa.me/{numero}?text={mensagem}");
            }
        }
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
