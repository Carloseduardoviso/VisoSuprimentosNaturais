using Microsoft.AspNetCore.Identity;
using VisoERP.Application.Interface.Auth;

namespace VisoERP.Infra.Auth.Services;

public sealed class AuthGateway(SignInManager<IdentityUser> signInManager) : IAuthGateway
{
    public async Task<ResultadoLogin> EntrarAsync(string email, string senha, bool lembrar,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await signInManager.PasswordSignInAsync(email, senha, lembrar, lockoutOnFailure: true);
        return result.Succeeded ? ResultadoLogin.Sucesso :
            result.IsLockedOut ? ResultadoLogin.Bloqueado : ResultadoLogin.Invalido;
    }

    public async Task SairAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await signInManager.SignOutAsync();
    }
}
