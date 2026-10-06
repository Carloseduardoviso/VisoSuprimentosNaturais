using VisoERP.Domain.Entities.Cadastros;
using VisoERP.Domain.Entities.Comercial;
using VisoERP.Infra.Helper;

namespace VisoERP.Tests;

public class SuprimentoTests
{
    [Fact]
    public void CustoDeCompraPodeSuperarPrecoDeVenda()
    {
        var suprimento = Suprimento.Criar("CAM-001", "Camomila", 10m, 11m, 1);

        Assert.Equal(10m, suprimento.PrecoCatalogo);
        Assert.Equal(11m, suprimento.PrecoComDesconto);
    }

    [Fact]
    public void CompraMinimaDeveSerPositiva()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Suprimento.Criar("CAM-001", "Camomila", 10m, 9m, 0));
    }

    [Fact]
    public void VendaUsaPrecoDeCatalogoComoReferenciaEPrecoEfetivo()
    {
        var venda = Venda.Criar(Guid.NewGuid(), 0m, 0m, 0, DateOnly.FromDateTime(DateTime.Today));
        venda.AdicionarItem(Guid.NewGuid(), 1m, 130m, 130m, false, 69m);

        var item = Assert.Single(venda.Itens);
        Assert.Equal(130m, item.PrecoCatalogo);
        Assert.Equal(130m, item.PrecoUnitario);
        Assert.Equal(69m, item.CustoUnitarioHistorico);
    }

    [Fact]
    public void PedidoAceitaCustoDeCompraAcimaDoPrecoDeVenda()
    {
        var pedido = PedidoFornecedor.Criar(Guid.NewGuid());

        pedido.AdicionarItem(Guid.NewGuid(), 2m, 10m, 11m);

        Assert.Equal(22m, pedido.Total);
    }

    [Fact]
    public void PedidoPermiteEditarDataDoPedido()
    {
        var dataOriginal = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero);
        var novaData = new DateTimeOffset(2026, 10, 5, 0, 0, 0, TimeSpan.Zero);
        var pedido = PedidoFornecedor.Criar(Guid.NewGuid(), dataOriginal);

        pedido.Editar(Guid.NewGuid(), novaData);

        Assert.Equal(novaData, pedido.DataCriacao);
    }

    [Theory]
    [InlineData("10,00", 10)]
    [InlineData("1.234,56", 1234.56)]
    [InlineData("10.00", 10)]
    [InlineData("1.234", 1.234)]
    public void ValorDecimalAceitaFormatoBrasileiroEInvariante(string informado, decimal esperado)
    {
        Assert.True(DecimalPtBrParser.TryParse(informado, out var valor));
        Assert.Equal(esperado, valor);
    }

    [Fact]
    public void ValorDecimalRejeitaAgrupamentoBrasileiroInvalido()
    {
        Assert.False(DecimalPtBrParser.TryParse("12.3,45", out _));
    }
}
