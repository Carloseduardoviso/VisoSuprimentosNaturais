namespace VisoERP.Application.DTOs.Comercial;

public sealed record ItemPedidoDto(Guid SuprimentoId, decimal Quantidade, decimal QuantidadeRecebida,
    decimal PrecoCatalogo, decimal PrecoComDesconto, decimal Total);

public sealed record CriarItemPedidoDto(Guid SuprimentoId, decimal Quantidade,
    decimal PrecoCatalogo, decimal PrecoComDesconto);

public sealed record CriarPedidoDto(Guid FornecedorId, IReadOnlyList<CriarItemPedidoDto> Itens, DateTimeOffset? DataPedido = null);

public sealed record PedidoFornecedorDto(Guid Id, Guid FornecedorId, DateTimeOffset DataCriacao,
    string Situacao, decimal Total, IReadOnlyList<ItemPedidoDto> Itens);
