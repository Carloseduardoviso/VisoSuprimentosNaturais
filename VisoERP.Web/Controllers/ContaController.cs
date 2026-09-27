using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisoERP.Application.Interface.Auth;
using VisoERP.Web.Models.Auth;

namespace VisoERP.Web.Controllers;

public sealed class ContaController(IContaAppService conta) : Controller
{
    [AllowAnonymous, HttpGet]
    public IActionResult Entrar(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [AllowAnonymous, HttpPost]
    public async Task<IActionResult> Entrar(LoginViewModel model, string? returnUrl,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);
        var resultado = await conta.EntrarAsync(model.Email, model.Senha, model.Lembrar, cancellationToken);
        if (resultado == ResultadoLogin.Sucesso)
            return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "/");
        ModelState.AddModelError(string.Empty, resultado == ResultadoLogin.Bloqueado
            ? "Conta temporariamente bloqueada." : "Credenciais inválidas.");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Sair(CancellationToken cancellationToken)
    {
        await conta.SairAsync(cancellationToken);
        return RedirectToAction(nameof(Entrar));
    }

    [AllowAnonymous, HttpGet]
    public IActionResult AcessoNegado() => View();
}
