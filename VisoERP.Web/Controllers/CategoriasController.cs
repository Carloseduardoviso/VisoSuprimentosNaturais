using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisoERP.Application.DTOs.Cadastros;
using VisoERP.Application.Interface.Cadastros;
using VisoERP.Web.Models.Cadastros;

namespace VisoERP.Web.Controllers;

[Authorize(Policy = "GerenciarCategorias")]
public sealed class CategoriasController(ICategoriaAppService categorias) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await categorias.ListarAsync(cancellationToken));

    [HttpGet]
    public IActionResult Novo() => View("Formulario", new CategoriaViewModel());

    [HttpGet]
    public async Task<IActionResult> Editar(Guid id, CancellationToken cancellationToken)
    {
        var categoria = await categorias.ObterAsync(id, cancellationToken);
        return categoria is null ? NotFound() : View("Formulario", new CategoriaViewModel
        {
            Id = categoria.Id, Nome = categoria.Nome, Ativa = categoria.Ativa
        });
    }

    [HttpPost]
    public async Task<IActionResult> Salvar(CategoriaViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View("Formulario", model);
        try
        {
            await categorias.SalvarAsync(model.Id,
                new SalvarCategoriaDto(model.Nome, model.Ativa), cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (ex is ArgumentException or KeyNotFoundException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Formulario", model);
        }
    }
}
