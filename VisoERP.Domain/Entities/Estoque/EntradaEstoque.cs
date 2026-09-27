namespace VisoERP.Domain.Entities.Estoque;

public sealed class EntradaEstoque : EntidadeBase
{
    private readonly List<ItemEntradaEstoque> _itens = [];
    private EntradaEstoque() { }
    public Guid? PedidoFornecedorId { get; private set; }
    public DateTimeOffset CriadaEm { get; private set; } = DateTimeOffset.UtcNow;
    public IReadOnlyCollection<ItemEntradaEstoque> Itens => _itens.AsReadOnly();
    public decimal TotalCusto => _itens.Sum(x => x.Quantidade * x.CustoUnitario);

    public static EntradaEstoque Criar(Guid? pedidoFornecedorId = null) =>
        new() { PedidoFornecedorId = pedidoFornecedorId };

    public void AdicionarItem(Guid suprimentoId, decimal quantidade, decimal custoUnitario,
        DateTimeOffset data, bool promocional, string? codigoLote, DateOnly? validade)
    {
        _itens.Add(ItemEntradaEstoque.Criar(Id, suprimentoId, quantidade, custoUnitario,
            data, promocional, codigoLote, validade));
    }

    public void Validar()
    {
        if (_itens.Count == 0) throw new InvalidOperationException("Informe ao menos um suprimento.");
        if (_itens.GroupBy(x => x.SuprimentoId).Any(x => x.Count() > 1))
            throw new InvalidOperationException("Cada suprimento deve aparecer uma vez por entrada.");
    }
}
