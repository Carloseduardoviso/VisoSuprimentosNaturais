namespace VisoERP.Domain.Entities.Estoque;

public sealed class ItemEntradaEstoque : EntidadeBase
{
    private ItemEntradaEstoque() { }
    public Guid EntradaEstoqueId { get; private set; }
    public Guid SuprimentoId { get; private set; }
    public decimal Quantidade { get; private set; }
    public decimal CustoUnitario { get; private set; }
    public DateTimeOffset Data { get; private set; }
    public bool Promocional { get; private set; }
    public string? CodigoLote { get; private set; }
    public DateOnly? Validade { get; private set; }

    internal static ItemEntradaEstoque Criar(Guid entradaId, Guid suprimentoId, decimal quantidade,
        decimal custoUnitario, DateTimeOffset data, bool promocional, string? codigoLote,
        DateOnly? validade)
    {
        if (suprimentoId == Guid.Empty) throw new ArgumentException("Suplemento alimentar inválido.", nameof(suprimentoId));
        if (quantidade <= 0) throw new ArgumentOutOfRangeException(nameof(quantidade));
        if (custoUnitario < 0) throw new ArgumentOutOfRangeException(nameof(custoUnitario));
        if (data == default) throw new ArgumentException("Data inválida.", nameof(data));
        if (codigoLote?.Trim().Length > 80) throw new ArgumentException("Lote muito longo.", nameof(codigoLote));
        if (validade is not null && string.IsNullOrWhiteSpace(codigoLote))
            throw new ArgumentException("Informe o lote quando houver validade.", nameof(codigoLote));
        if (validade < DateOnly.FromDateTime(data.Date))
            throw new ArgumentException("Validade anterior à entrada.", nameof(validade));
        return new ItemEntradaEstoque
        {
            EntradaEstoqueId = entradaId, SuprimentoId = suprimentoId, Quantidade = quantidade,
            CustoUnitario = custoUnitario, Data = data, Promocional = promocional,
            CodigoLote = string.IsNullOrWhiteSpace(codigoLote) ? null : codigoLote.Trim(), Validade = validade
        };
    }
}
