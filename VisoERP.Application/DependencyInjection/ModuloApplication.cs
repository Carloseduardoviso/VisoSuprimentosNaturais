using Microsoft.Extensions.DependencyInjection;
using VisoERP.Application.AutoMapper.Cadastros;
using VisoERP.Application.AppService.Auth;
using VisoERP.Application.AppService.Cadastros;
using VisoERP.Application.Interface.Auth;
using VisoERP.Application.Interface.Cadastros;
using VisoERP.Application.AppService.Comercial;
using VisoERP.Application.AppService.Estoque;
using VisoERP.Application.Interface.Comercial;
using VisoERP.Application.Interface.Estoque;

namespace VisoERP.Application.DependencyInjection;

public static class ModuloApplication
{
    public static IServiceCollection RegistrarApplication(this IServiceCollection services, string? autoMapperLicenseKey)
    {
        services.AddScoped<IContaAppService, ContaAppService>();
        services.AddScoped<ISuprimentoAppService, SuprimentoAppService>();
        services.AddScoped<ICategoriaAppService, CategoriaAppService>();
        services.AddScoped<IClienteAppService, ClienteAppService>();
        services.AddScoped<IFornecedorAppService, FornecedorAppService>();
        services.AddScoped<IPedidoFornecedorAppService, PedidoFornecedorAppService>();
        services.AddScoped<IEntradaEstoqueAppService, EntradaEstoqueAppService>();
        services.AddAutoMapper(config =>
        {
            if (!string.IsNullOrWhiteSpace(autoMapperLicenseKey))
                config.LicenseKey = autoMapperLicenseKey;
        }, typeof(CategoriaProfile).Assembly);
        return services;
    }
}
