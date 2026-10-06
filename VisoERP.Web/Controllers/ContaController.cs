using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VisoERP.Application.Interface.Auth;
using VisoERP.Domain.Entities;
using VisoERP.Infra.Data.Context;
using VisoERP.Web.Models.Auth;

namespace VisoERP.Web.Controllers;

public sealed class ContaController(IContaAppService conta, VisoErpDbContext db,
    IPasswordHasher<Usuario> hasher) : Controller
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
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(model);
        var resultado = await conta.EntrarAsync(model.Cpf, model.Senha, model.Lembrar, cancellationToken);
        if (resultado == ResultadoLogin.Sucesso)
            return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "/");
        ModelState.AddModelError(string.Empty, resultado == ResultadoLogin.Bloqueado
            ? "Conta temporariamente bloqueada." : "Credenciais inválidas.");
        ViewData["ReturnUrl"] = returnUrl;
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

    [AllowAnonymous, HttpGet]
    public IActionResult CadastroInterno() => View(new CadastroUsuarioViewModel());

    [AllowAnonymous, HttpPost]
    public async Task<IActionResult> CadastroInterno(CadastroUsuarioViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);
        var cpf = Usuario.NormalizarCpf(model.Cpf);
        if (cpf.Length != 11 || !cpf.All(char.IsDigit))
        {
            ModelState.AddModelError(nameof(model.Cpf), "Informe um CPF válido.");
            return View(model);
        }
        if (await db.Usuarios.AnyAsync(x => x.Cpf == cpf, cancellationToken))
        {
            ModelState.AddModelError(nameof(model.Cpf), "Este CPF já está cadastrado.");
            return View(model);
        }

        var usuario = new Usuario { NomeCompleto = model.NomeCompleto.Trim(), Cpf = cpf };
        usuario.SenhaHash = hasher.HashPassword(usuario, model.Senha);
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync(cancellationToken);
        TempData["Mensagem"] = "Usuário cadastrado. Faça login para continuar.";
        return RedirectToAction(nameof(Entrar));
    }
}
