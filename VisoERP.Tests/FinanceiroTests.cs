using VisoERP.Domain.Entities.Financeiro;

namespace VisoERP.Tests;

public sealed class FinanceiroTests
{
    [Fact]
    public void ContaPagarAceitaPagamentosParciaisSemExcederSaldo()
    {
        var conta = ContaPagar.Criar(Guid.NewGuid(), null, "Compra de estoque", 100,
            new DateOnly(2026, 11, 10));
        conta.RegistrarPagamento(30);
        Assert.Equal(70, conta.Saldo);
        Assert.Throws<ArgumentOutOfRangeException>(() => conta.RegistrarPagamento(71));
        conta.RegistrarPagamento(70);
        Assert.Equal(0, conta.Saldo);
    }

    [Fact]
    public void DespesaEInvestimentoValidamValores()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Investimento.Criar("Capital inicial", 0, DateOnly.FromDateTime(DateTime.Today)));
        var despesa = Despesa.Criar("Aluguel", 500, DateOnly.FromDateTime(DateTime.Today));
        despesa.MarcarPaga(DateTimeOffset.UtcNow);
        Assert.NotNull(despesa.PagaEm);
        Assert.Throws<InvalidOperationException>(() => despesa.MarcarPaga(DateTimeOffset.UtcNow));
    }
}
