using VisoERP.Domain.Enums;

namespace VisoERP.Domain.Entities.Comercial;

public sealed class PedidoFornecedor : EntidadeBase
{
    private readonly List<ItemPedidoFornecedor> _itens = [];
    private PedidoFornecedor() { }

    public Guid FornecedorId { get; private set; }
    public DateTimeOffset DataCriacao { get; private set; } = DateTimeOffset.UtcNow;
    public SituacaoPedido Situacao { get; private set; } = SituacaoPedido.Pendente;
    public IReadOnlyCollection<ItemPedidoFornecedor> Itens => _itens.AsReadOnly();
    public decimal Total => _itens.Sum(x => x.Total);

    public static PedidoFornecedor Criar(Guid fornecedorId)
    {
        if (fornecedorId == Guid.Empty) throw new ArgumentException("Fornecedor inválido.", nameof(fornecedorId));
        return new PedidoFornecedor { FornecedorId = fornecedorId };
    }

    public void AdicionarItem(Guid suprimentoId, decimal quantidade, decimal precoCatalogo,
        decimal precoComDesconto)
    {
        if (Situacao != SituacaoPedido.Pendente) throw new InvalidOperationException("Pedido não está aberto.");
        if (_itens.Any(x => x.SuprimentoId == suprimentoId))
            throw new InvalidOperationException("Suprimento já consta no pedido.");
        _itens.Add(ItemPedidoFornecedor.Criar(Id, suprimentoId, quantidade, precoCatalogo, precoComDesconto));
    }

    public void Receber(Guid suprimentoId, decimal quantidade)
    {
        if (Situacao is SituacaoPedido.Recebido or SituacaoPedido.Cancelado)
            throw new InvalidOperationException("Pedido não aceita recebimentos.");
        var item = _itens.SingleOrDefault(x => x.SuprimentoId == suprimentoId)
            ?? throw new KeyNotFoundException("Item não encontrado no pedido.");
        item.Receber(quantidade);
        Situacao = _itens.All(x => x.QuantidadeRecebida == x.Quantidade)
            ? SituacaoPedido.Recebido : SituacaoPedido.Parcial;
    }

    public void RegistrarFalta(Guid suprimentoId, decimal quantidade) =>
        (_itens.SingleOrDefault(x => x.SuprimentoId == suprimentoId) ?? throw new KeyNotFoundException()).RegistrarFalta(quantidade);

    public void Cancelar()
    {
        if (_itens.Any(x => x.QuantidadeRecebida > 0))
            throw new InvalidOperationException("Pedido recebido não pode ser cancelado.");
        Situacao = SituacaoPedido.Cancelado;
    }
}
