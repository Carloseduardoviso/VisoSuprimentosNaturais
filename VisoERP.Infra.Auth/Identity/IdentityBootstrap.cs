using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VisoERP.Infra.Auth.Identity;

public static class IdentityBootstrap
{
    public static async Task InicializarAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        using var scope = serviceProvider.CreateScope();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var nome in new[] { "Administrador", "Estoque", "Vendas", "Financeiro" })
        {
            if (await roles.RoleExistsAsync(nome)) continue;
            var result = await roles.CreateAsync(new IdentityRole(nome));
            if (!result.Succeeded)
                throw new InvalidOperationException("Falha ao criar fun��o: " + nome);
        }

        var email = configuration["AdminBootstrap:Email"];
        var senha = configuration["AdminBootstrap:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha)) return;

        var users = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var usuario = await users.FindByEmailAsync(email);
        if (usuario is null)
        {
            usuario = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await users.CreateAsync(usuario, senha);
            if (!result.Succeeded)
                throw new InvalidOperationException("Falha ao criar administrador inicial: " +
                    string.Join("; ", result.Errors.Select(x => x.Description)));
            var roleResult = await users.AddToRoleAsync(usuario, "Administrador");
            if (!roleResult.Succeeded) throw new InvalidOperationException("Falha ao atribuir fun��o Administrador.");
        }
        else if (!await users.IsInRoleAsync(usuario, "Administrador"))
            throw new InvalidOperationException("O e-mail de bootstrap j� pertence a um usu�rio sem permiss�o administrativa.");
    }
}
