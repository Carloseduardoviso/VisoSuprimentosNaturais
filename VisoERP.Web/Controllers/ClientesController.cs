using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisoERP.Application.DTOs.Cadastros;
using VisoERP.Application.Interface.Cadastros;
using VisoERP.Web.Models.Cadastros;

namespace VisoERP.Web.Controllers;

[Authorize(Policy = "GerenciarClientes")]
public sealed class ClientesController(IClienteAppService clientes) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await clientes.ListarAsync(cancellationToken));

    [HttpGet]
    public IActionResult Novo() => View("Formulario", new ClienteViewModel());

    [HttpGet]
    public async Task<IActionResult> Editar(Guid id, CancellationToken cancellationToken)
    {
        var cliente = await clientes.ObterAsync(id, cancellationToken);
        return cliente is null ? NotFound() : View("Formulario", new ClienteViewModel
        {
            Id = cliente.Id, Nome = cliente.Nome, Documento = cliente.Documento,
            CpfCnpj = cliente.Documento, Cep = cliente.Cep, Numero = cliente.Numero, Endereco = cliente.Endereco,
            Email = cliente.Email, Telefone = cliente.Telefone, Ativo = cliente.Ativo
        });
    }

    [HttpPost]
    public async Task<IActionResult> Salvar(ClienteViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View("Formulario", model);
        try
        {
            await clientes.SalvarAsync(model.Id,
                new SalvarClienteDto(model.Nome, model.CpfCnpj, model.Email,
                    model.Telefone, model.Ativo, model.Cep, model.Numero, model.Endereco), cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (ex is ArgumentException or KeyNotFoundException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Formulario", model);
        }
    }
}
