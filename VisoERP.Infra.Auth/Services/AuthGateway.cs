using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VisoERP.Application.Interface.Auth;
using VisoERP.Domain.Entities;
using VisoERP.Infra.Data.Context;

namespace VisoERP.Infra.Auth.Services;

public sealed class AuthGateway(
    VisoErpDbContext db,
    IHttpContextAccessor httpContext,
    IPasswordHasher<Usuario> hasher) : IAuthGateway
{
    public async Task<ResultadoLogin> EntrarAsync(string cpf, string senha, bool lembrar,
        CancellationToken cancellationToken)
    {
        var normalizado = Usuario.NormalizarCpf(cpf);
        var usuario = await db.Usuarios.SingleOrDefaultAsync(x => x.Cpf == normalizado, cancellationToken);
        if (usuario is null || !usuario.Ativo) return ResultadoLogin.Invalido;

        var verificacao = hasher.VerifyHashedPassword(usuario, usuario.SenhaHash, senha);
        if (verificacao == PasswordVerificationResult.Failed) return ResultadoLogin.Invalido;

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.NomeCompleto),
            new Claim("cpf", usuario.Cpf)
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme));
        await httpContext.HttpContext!.SignInAsync(IdentityConstants.ApplicationScheme, principal,
            new AuthenticationProperties
            {
                IsPersistent = lembrar,
                AllowRefresh = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(6)
            });
        return ResultadoLogin.Sucesso;
    }

    public Task SairAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return httpContext.HttpContext!.SignOutAsync(IdentityConstants.ApplicationScheme);
    }
}
