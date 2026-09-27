namespace VisoERP.Domain.Entities.Financeiro;

public sealed class PagamentoContaPagar : EntidadeBase
{
    private PagamentoContaPagar() { }
    public Guid ContaPagarId { get; private set; }
    public decimal Valor { get; private set; }
    public DateTimeOffset Data { get; private set; }

    public static PagamentoContaPagar Criar(Guid contaPagarId, decimal valor, DateTimeOffset data)
    {
        if (contaPagarId == Guid.Empty || valor <= 0 || data == default)
            throw new ArgumentException("Pagamento inválido.");
        return new PagamentoContaPagar { ContaPagarId = contaPagarId, Valor = valor, Data = data };
    }
}
