namespace VisoERP.Domain.Entities.Comercial;

public sealed class ItemVenda : EntidadeBase
{
    private ItemVenda() { }
    public Guid VendaId { get; private set; }
    public Guid SuprimentoId { get; private set; }
    public decimal Quantidade { get; private set; }
    public decimal PrecoCatalogo { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public bool Promocional { get; private set; }
    public DateTimeOffset Data { get; private set; }
    public decimal CustoUnitarioHistorico { get; private set; }
    public decimal Total => Math.Round(Quantidade * PrecoUnitario, 2, MidpointRounding.AwayFromZero);
    public decimal CustoTotal => Math.Round(Quantidade * CustoUnitarioHistorico, 2, MidpointRounding.AwayFromZero);

    internal static ItemVenda Criar(Guid vendaId, Guid suprimentoId, decimal quantidade,
        decimal precoCatalogo, decimal precoUnitario, bool promocional, decimal custoUnitario,
        DateTimeOffset data)
    {
        if (suprimentoId == Guid.Empty) throw new ArgumentException("Suprimento inválido.");
        if (quantidade <= 0) throw new ArgumentOutOfRangeException(nameof(quantidade));
        if (precoCatalogo < 0 || precoUnitario < 0 || precoUnitario > precoCatalogo)
            throw new ArgumentException("Preço inválido.");
        if (custoUnitario < 0) throw new ArgumentOutOfRangeException(nameof(custoUnitario));
        if (data == default) throw new ArgumentException("Data inválida.");
        return new ItemVenda { VendaId = vendaId, SuprimentoId = suprimentoId,
            Quantidade = quantidade, PrecoCatalogo = precoCatalogo, PrecoUnitario = precoUnitario,
            Promocional = promocional, CustoUnitarioHistorico = custoUnitario, Data = data };
    }
}
