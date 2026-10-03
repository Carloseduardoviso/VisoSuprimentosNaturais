using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VisoERP.Application.DependencyInjection;
using VisoERP.Application.Interface.Auth;
using VisoERP.Domain.Entities;
using VisoERP.Domain.Interfaces.Repositories;
using VisoERP.Domain.Interfaces.Repositories.Cadastros;
using VisoERP.Domain.Interfaces.Repositories.Comercial;
using VisoERP.Domain.Interfaces.Repositories.Estoque;
using VisoERP.Infra.Data.Context;
using VisoERP.Infra.Data.Repositories;
using VisoERP.Infra.Data.Repositories.Cadastros;
using VisoERP.Infra.Data.Repositories.Comercial;
using VisoERP.Infra.Data.Repositories.Estoque;
using VisoERP.Infra.Auth.Services;
using VisoERP.Infra.Auth.Identity;
using VisoERP.Infra.Helper.Settings;

namespace VisoERP.Infra.Ioc;

public static class Modulo
{
    public static Task InicializarIdentidadeAsync(IServiceProvider services, IConfiguration configuration) =>
        IdentityBootstrap.InicializarAsync(services, configuration);

    public static IServiceCollection RegistrarServicos(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(SqlServerSettings.ConnectionStringName);
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException($"ConnectionStrings:{SqlServerSettings.ConnectionStringName} n�o configurada.");

        services.AddDbContext<VisoErpDbContext>(options => options.UseSqlServer(connectionString));
        services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 12;
            options.Password.RequireNonAlphanumeric = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
        }).AddEntityFrameworkStores<VisoErpDbContext>().AddDefaultTokenProviders();
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Conta/Entrar";
            options.AccessDeniedPath = "/Conta/AcessoNegado";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
            options.ExpireTimeSpan = TimeSpan.FromHours(6);
            options.SlidingExpiration = false;
        });
        services.AddHttpContextAccessor();
        services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
        services.AddScoped<IAuthGateway, AuthGateway>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<ISuprimentoRepository, SuprimentoRepository>();
        services.AddScoped<IPedidoFornecedorRepository, PedidoFornecedorRepository>();
        services.AddScoped<IVendaRepository, VendaRepository>();
        services.AddScoped<IEstoqueRepository, EstoqueRepository>();
        services.AddScoped<IUnitOfWork, VisoERP.Infra.Data.UnitOfWork.UnitOfWork>();
        services.RegistrarApplication(configuration["AutoMapper:LicenseKey"]);
        return services;
    }
}
