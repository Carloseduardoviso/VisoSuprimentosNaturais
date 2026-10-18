namespace VisoERP.Domain.Entities.Comercial;

public sealed class ItemPedidoFornecedor : EntidadeBase
{
    private ItemPedidoFornecedor() { }

    public Guid PedidoFornecedorId { get; private set; }
    public Guid SuprimentoId { get; private set; }
    public decimal Quantidade { get; private set; }
    public decimal QuantidadeRecebida { get; private set; }
    public decimal QuantidadeNaoRecebida { get; private set; }
    public decimal PrecoCatalogo { get; private set; }
    public decimal PrecoComDesconto { get; private set; }
    public decimal Total => Quantidade * PrecoComDesconto;

    internal static ItemPedidoFornecedor Criar(Guid pedidoId, Guid suprimentoId, decimal quantidade,
        decimal precoCatalogo, decimal precoComDesconto)
    {
        if (suprimentoId == Guid.Empty) throw new ArgumentException("Suplemento alimentar inválido.", nameof(suprimentoId));
        if (quantidade <= 0) throw new ArgumentOutOfRangeException(nameof(quantidade));
        if (precoCatalogo < 0) throw new ArgumentOutOfRangeException(nameof(precoCatalogo));
        if (precoComDesconto < 0)
            throw new ArgumentOutOfRangeException(nameof(precoComDesconto));
        return new ItemPedidoFornecedor
        {
            PedidoFornecedorId = pedidoId, SuprimentoId = suprimentoId,
            Quantidade = quantidade, PrecoCatalogo = precoCatalogo, PrecoComDesconto = precoComDesconto
        };
    }

    internal void Receber(decimal quantidade)
    {
        if (quantidade <= 0) throw new ArgumentOutOfRangeException(nameof(quantidade));
        if (QuantidadeRecebida + quantidade > Quantidade)
            throw new InvalidOperationException("Recebimento excede a quantidade pedida.");
        QuantidadeRecebida += quantidade;
    }

    internal void RegistrarFalta(decimal quantidade)
    {
        if (quantidade < 0 || QuantidadeRecebida + quantidade > Quantidade)
            throw new ArgumentOutOfRangeException(nameof(quantidade));
        QuantidadeNaoRecebida = quantidade;
    }

    internal void EstornarRecebimento(decimal quantidade)
    {
        if (quantidade <= 0 || quantidade > QuantidadeRecebida)
            throw new ArgumentOutOfRangeException(nameof(quantidade));
        QuantidadeRecebida -= quantidade;
    }

    internal void Atualizar(decimal quantidade, decimal precoCatalogo, decimal precoComDesconto)
    {
        if (quantidade < QuantidadeRecebida)
            throw new InvalidOperationException("A quantidade não pode ser menor que a quantidade já recebida.");
        if (quantidade <= 0) throw new ArgumentOutOfRangeException(nameof(quantidade));
        if (precoCatalogo < 0) throw new ArgumentOutOfRangeException(nameof(precoCatalogo));
        if (precoComDesconto < 0) throw new ArgumentOutOfRangeException(nameof(precoComDesconto));

        Quantidade = quantidade;
        PrecoCatalogo = precoCatalogo;
        PrecoComDesconto = precoComDesconto;
    }
}
