using VisoERP.Domain.Entities.Estoque;
using VisoERP.Domain.Entities.Comercial;
using VisoERP.Domain.Enums;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using VisoERP.Application.DependencyInjection;
using VisoERP.Application.DTOs.Comercial;
using VisoERP.Application.DTOs.Estoque;

namespace VisoERP.Tests;

public class EstoqueEPedidoTests
{
    [Fact]
    public void EntradaAtualizaCustoMedioPonderado()
    {
        var saldo = EstoqueProduto.Criar(Guid.NewGuid());
        saldo.RegistrarEntrada(10m, 5m);
        saldo.RegistrarEntrada(10m, 7m);
        Assert.Equal(20m, saldo.Quantidade);
        Assert.Equal(6m, saldo.CustoMedio);
    }

    [Fact]
    public void PedidoControlaRecebimentoParcial()
    {
        var produtoId = Guid.NewGuid();
        var pedido = PedidoFornecedor.Criar(Guid.NewGuid());
        pedido.AdicionarItem(produtoId, 5m, 10m, 9m);
        pedido.Receber(produtoId, 2m);
        Assert.Equal(SituacaoPedido.Parcial, pedido.Situacao);
        Assert.Throws<InvalidOperationException>(() => pedido.Receber(produtoId, 4m));
        pedido.Receber(produtoId, 3m);
        Assert.Equal(SituacaoPedido.Recebido, pedido.Situacao);
    }

    [Fact]
    public void EntradaExigeItensUnicosELoteValido()
    {
        var produto = Guid.NewGuid();
        var entrada = EntradaEstoque.Criar();
        Assert.Throws<InvalidOperationException>(() => entrada.Validar());
        var data = DateTimeOffset.UtcNow;
        Assert.Throws<ArgumentException>(() => entrada.AdicionarItem(produto, 1, 2, data, false,
            "L-1", DateOnly.FromDateTime(data.Date).AddDays(-1)));
        entrada.AdicionarItem(produto, 1, 2, data, false, "L-1",
            DateOnly.FromDateTime(data.Date).AddDays(90));
        entrada.AdicionarItem(produto, 2, 3, data, false, null, null);
        Assert.Throws<InvalidOperationException>(() => entrada.Validar());
    }

    [Fact]
    public void PerfisMapeiamPedidosEEntradasComItens()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.RegistrarApplication(null);
        using var provider = services.BuildServiceProvider();
        var mapper = provider.GetRequiredService<IMapper>();
        var produtoId = Guid.NewGuid();
        var pedido = PedidoFornecedor.Criar(Guid.NewGuid());
        pedido.AdicionarItem(produtoId, 2, 10, 9);
        var dtoPedido = mapper.Map<PedidoFornecedorDto>(pedido);
        Assert.Single(dtoPedido.Itens);
        Assert.Equal(18, dtoPedido.Total);

        var entrada = EntradaEstoque.Criar(pedido.Id);
        entrada.AdicionarItem(produtoId, 2, 9, DateTimeOffset.UtcNow, false, null, null);
        var dtoEntrada = mapper.Map<EntradaEstoqueDto>(entrada);
        Assert.Single(dtoEntrada.Itens);
        Assert.Equal(18, dtoEntrada.TotalCusto);
    }
}
