using VisoERP.Domain.Entities.Comercial;

namespace VisoERP.Tests;

public sealed class VendaTests
{
    [Fact]
    public void ParcelamentoDistribuiCentavosSemPerderSaldo()
    {
        var venda = Venda.Criar(Guid.NewGuid(), 0, 0, 3, new DateOnly(2026, 10, 10));
        venda.AdicionarItem(Guid.NewGuid(), 1, 10, 10, false, 4);
        venda.Finalizar();
        Assert.Equal(3, venda.Parcelas.Count);
        Assert.Equal(10m, venda.Parcelas.Sum(x => x.Valor));
        Assert.Equal(new[] { 3.34m, 3.33m, 3.33m }, venda.Parcelas.Select(x => x.Valor));
        Assert.Equal(new DateOnly(2026, 12, 10), venda.Parcelas.Last().Vencimento);
    }

    [Fact]
    public void EntradaFinanceiraNaoReduzFaturamento()
    {
        var venda = Venda.Criar(Guid.NewGuid(), 2, 3, 1, new DateOnly(2026, 10, 10));
        venda.AdicionarItem(Guid.NewGuid(), 1, 20, 18, true, 7);
        venda.Finalizar();
        Assert.Equal(16m, venda.Total);
        Assert.Equal(3m, venda.ValorRecebido);
        Assert.Equal(13m, Assert.Single(venda.Parcelas).Valor);
        Assert.Equal(7m, venda.CustoTotal);
    }

    [Fact]
    public void VendaRejeitaDescontoExcessivoEParcelasSemValor()
    {
        var venda = Venda.Criar(Guid.NewGuid(), 21, 0, 1, new DateOnly(2026, 10, 10));
        venda.AdicionarItem(Guid.NewGuid(), 1, 20, 20, false, 5);
        Assert.Throws<InvalidOperationException>(() => venda.Finalizar());

        var pequena = Venda.Criar(Guid.NewGuid(), 0, 0, 12, new DateOnly(2026, 10, 10));
        pequena.AdicionarItem(Guid.NewGuid(), 1, .01m, .01m, false, 0);
        Assert.Throws<InvalidOperationException>(() => pequena.Finalizar());
    }
}
