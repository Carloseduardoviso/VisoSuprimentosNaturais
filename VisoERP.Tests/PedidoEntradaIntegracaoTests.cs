using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VisoERP.Application.DTOs.Comercial;
using VisoERP.Application.DTOs.Estoque;
using VisoERP.Application.Interface.Comercial;
using VisoERP.Application.Interface.Estoque;
using VisoERP.Domain.Entities.Cadastros;
using VisoERP.Infra.Data.Context;
using VisoERP.Infra.Ioc;

namespace VisoERP.Tests;

public sealed class PedidoEntradaIntegracaoTests
{
    [Fact]
    [Trait("Category", "SqlServer")]
    public async Task RecebimentoParcialSoAlteraEstoqueAoConfirmar()
    {
        var nomeBanco = "VisoERP_Test_" + Guid.NewGuid().ToString("N");
        var connectionString = $"Server=(localdb)\\MSSQLLocalDB;Database={nomeBanco};Trusted_Connection=True;TrustServerCertificate=True";
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:VisoERP"] = connectionString
        }).Build();
        var services = new ServiceCollection();
        services.AddLogging();
        Modulo.RegistrarServicos(services, config);
        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<VisoErpDbContext>();
        try
        {
            await db.Database.MigrateAsync();
            var fornecedor = Fornecedor.Criar("Fornecedor teste", null, null, null);
            var produto = Suprimento.Criar("TEST-" + Guid.NewGuid().ToString("N"), "Produto teste", 20, 18, 1);
            db.Fornecedores.Add(fornecedor);
            db.Suprimentos.Add(produto);
            await db.SaveChangesAsync();

            var pedidos = scope.ServiceProvider.GetRequiredService<IPedidoFornecedorAppService>();
            var entradas = scope.ServiceProvider.GetRequiredService<IEntradaEstoqueAppService>();
            var pedidoId = await pedidos.CriarAsync(new CriarPedidoDto(fornecedor.Id,
                [new CriarItemPedidoDto(produto.Id, 5, 12, 10)]), CancellationToken.None);
            Assert.Empty(await entradas.ListarSaldosAsync(CancellationToken.None));

            await entradas.ConfirmarAsync(new CriarEntradaDto(pedidoId,
                [new ItemEntradaDto(produto.Id, 2, 10, DateTimeOffset.UtcNow, false, "L-1",
                    DateOnly.FromDateTime(DateTime.Today.AddMonths(6)))]), CancellationToken.None);
            Assert.Equal("Parcial", (await pedidos.ObterAsync(pedidoId, CancellationToken.None))!.Situacao);
            var saldo = Assert.Single(await entradas.ListarSaldosAsync(CancellationToken.None));
            Assert.Equal(2, saldo.Quantidade);

            await entradas.ConfirmarAsync(new CriarEntradaDto(pedidoId,
                [new ItemEntradaDto(produto.Id, 3, 12, DateTimeOffset.UtcNow, false, null, null)]), CancellationToken.None);
            Assert.Equal("Recebido", (await pedidos.ObterAsync(pedidoId, CancellationToken.None))!.Situacao);
            saldo = Assert.Single(await entradas.ListarSaldosAsync(CancellationToken.None));
            Assert.Equal(5, saldo.Quantidade);
            Assert.Equal(11.2m, saldo.CustoMedio);
        }
        finally
        {
            await db.Database.EnsureDeletedAsync();
        }
    }
}
