using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VisoERP.Application.DTOs.Cadastros;
using VisoERP.Application.Interface.Cadastros;
using VisoERP.Web.Models.Cadastros;
using VisoERP.Web.Services.Cadastros;

namespace VisoERP.Web.Controllers;

[Authorize(Policy = "GerenciarSuprimentos")]
public sealed class SuprimentosController(ISuprimentoAppService suprimentos,
    ImagemSuprimentoService imagens, ILogger<SuprimentosController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await suprimentos.ListarAsync(cancellationToken));

    [HttpGet]
    public async Task<IActionResult> Novo(CancellationToken cancellationToken)
    {
        await CarregarCategoriasAsync(cancellationToken);
        return View("Formulario", new SuprimentoViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> Editar(Guid id, CancellationToken cancellationToken)
    {
        var produto = await suprimentos.ObterAsync(id, cancellationToken);
        if (produto is null) return NotFound();
        await CarregarCategoriasAsync(cancellationToken);
        return View("Formulario", new SuprimentoViewModel
        {
            Id = produto.Id, CodigoInterno = produto.CodigoInterno, Nome = produto.Nome,
            PrecoCatalogo = produto.PrecoCatalogo, PrecoComDesconto = produto.PrecoComDesconto,
            PorcentagemDesconto = produto.PrecoCatalogo == 0 ? 0 : Math.Round((1 - produto.PrecoComDesconto / produto.PrecoCatalogo) * 100, 2),
            Descricao = produto.Descricao, FormaDeUso = produto.FormaDeUso,
            QuantidadeMinimaCompra = produto.QuantidadeMinimaCompra, EstoqueMinimo = produto.EstoqueMinimo,
            CategoriaId = produto.CategoriaId, Ativo = produto.Ativo
        });
    }

    [HttpPost, RequestSizeLimit(3 * 1024 * 1024)]
    public async Task<IActionResult> Salvar(SuprimentoViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await CarregarCategoriasAsync(cancellationToken);
            return View("Formulario", model);
        }

        string? imagemNova = null;
        try
        {
            if (model.Imagem is not null)
                imagemNova = await imagens.SalvarAsync(model.Imagem, cancellationToken);
            await suprimentos.SalvarAsync(model.Id, new SalvarSuprimentoDto(
                model.CodigoInterno, model.Nome, model.PrecoCatalogo, model.PrecoComDesconto,
                model.QuantidadeMinimaCompra, model.EstoqueMinimo, model.CategoriaId,
                model.Descricao, model.FormaDeUso, imagemNova, model.Ativo), cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (ex is ArgumentException or KeyNotFoundException)
        {
            if (imagemNova is not null) imagens.Excluir(imagemNova);
            ModelState.AddModelError(string.Empty, ex.Message);
            await CarregarCategoriasAsync(cancellationToken);
            return View("Formulario", model);
        }
        catch (Exception ex)
        {
            if (imagemNova is not null) imagens.Excluir(imagemNova);
            logger.LogError(ex, "Erro ao salvar suplemento alimentar. Id: {SuprimentoId}", model.Id);
            throw;
        }
    }

    [HttpGet]
    public async Task<IActionResult> Imagem(Guid id, CancellationToken cancellationToken)
    {
        var produto = await suprimentos.ObterAsync(id, cancellationToken);
        var caminho = imagens.CaminhoParaLeitura(produto?.ImagemCaminho);
        if (caminho is null) return NotFound();
        var tipo = Path.GetExtension(caminho).ToLowerInvariant() switch
        {
            ".png" => "image/png", ".webp" => "image/webp", _ => "image/jpeg"
        };
        return PhysicalFile(caminho, tipo);
    }

    private async Task CarregarCategoriasAsync(CancellationToken cancellationToken) =>
        ViewBag.Categorias = new SelectList(await suprimentos.ListarCategoriasAsync(cancellationToken),
            "Id", "Nome");
}
