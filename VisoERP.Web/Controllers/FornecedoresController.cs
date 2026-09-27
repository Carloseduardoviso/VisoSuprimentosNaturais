using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisoERP.Application.DTOs.Cadastros;
using VisoERP.Application.Interface.Cadastros;
using VisoERP.Web.Models.Cadastros;

namespace VisoERP.Web.Controllers;

[Authorize(Policy = "GerenciarFornecedores")]
public sealed class FornecedoresController(IFornecedorAppService fornecedores) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await fornecedores.ListarAsync(cancellationToken));

    [HttpGet]
    public IActionResult Novo() => View("Formulario", new FornecedorViewModel());

    [HttpGet]
    public async Task<IActionResult> Editar(Guid id, CancellationToken cancellationToken)
    {
        var fornecedor = await fornecedores.ObterAsync(id, cancellationToken);
        return fornecedor is null ? NotFound() : View("Formulario", new FornecedorViewModel
        {
            Id = fornecedor.Id, Nome = fornecedor.Nome, Documento = fornecedor.Documento,
            Email = fornecedor.Email, Telefone = fornecedor.Telefone, Ativo = fornecedor.Ativo
        });
    }

    [HttpPost]
    public async Task<IActionResult> Salvar(FornecedorViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View("Formulario", model);
        try
        {
            await fornecedores.SalvarAsync(model.Id,
                new SalvarFornecedorDto(model.Nome, model.Documento, model.Email,
                    model.Telefone, model.Ativo), cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (ex is ArgumentException or KeyNotFoundException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Formulario", model);
        }
    }
}
