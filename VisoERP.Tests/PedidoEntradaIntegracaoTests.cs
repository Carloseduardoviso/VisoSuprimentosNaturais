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
            var cliente = Cliente.Criar("Cliente teste", null, null, null);
            var produto = Suprimento.Criar("TEST-" + Guid.NewGuid().ToString("N"), "Produto teste", 20, 18, 1);
            db.Fornecedores.Add(fornecedor);
            db.Clientes.Add(cliente);
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

            var vendas = scope.ServiceProvider.GetRequiredService<IVendaAppService>();
            var vendaId = await vendas.RegistrarAsync(new CriarVendaDto(cliente.Id, 0, 5, 3,
                DateOnly.FromDateTime(DateTime.Today.AddMonths(1)),
                [new CriarItemVendaDto(produto.Id, 2, 20, 20, false, DateTimeOffset.UtcNow)]),
                CancellationToken.None);
            var venda = (await vendas.ObterAsync(vendaId, CancellationToken.None))!;
            Assert.Equal(40m, venda.Total);
            Assert.Equal(5m, venda.ValorRecebido);
            Assert.Equal(22.4m, venda.CustoTotal);
            Assert.Equal(35m, venda.Parcelas.Sum(x => x.Valor));
            Assert.Equal(3m, Assert.Single(await entradas.ListarSaldosAsync(CancellationToken.None)).Quantidade);

            var parcela = venda.Parcelas.First();
            await vendas.RegistrarPagamentoAsync(vendaId, parcela.Id, parcela.Valor,
                DateTimeOffset.UtcNow, CancellationToken.None);
            venda = (await vendas.ObterAsync(vendaId, CancellationToken.None))!;
            Assert.Equal(5m + parcela.Valor, venda.ValorRecebido);
            Assert.Equal(2, await db.RecebimentosVendas.CountAsync());

            await Assert.ThrowsAsync<InvalidOperationException>(() => vendas.RegistrarAsync(
                new CriarVendaDto(cliente.Id, 0, 0, 1,
                    DateOnly.FromDateTime(DateTime.Today.AddMonths(1)),
                    [new CriarItemVendaDto(produto.Id, 4, 20, 20, false, DateTimeOffset.UtcNow)]),
                CancellationToken.None));
            Assert.Equal(3m, await db.EstoquesProdutos.AsNoTracking()
                .Where(x => x.SuprimentoId == produto.Id).Select(x => x.Quantidade).SingleAsync());
        }
        finally
        {
            await db.Database.EnsureDeletedAsync();
        }
    }
}
