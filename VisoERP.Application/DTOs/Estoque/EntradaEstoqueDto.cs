namespace VisoERP.Application.DTOs.Estoque;

public sealed record ItemEntradaDto(Guid SuprimentoId, decimal Quantidade, decimal CustoUnitario,
    DateTimeOffset Data, bool Promocional, string? CodigoLote, DateOnly? Validade);

public sealed record CriarEntradaDto(Guid? PedidoFornecedorId, IReadOnlyList<ItemEntradaDto> Itens);

public sealed record SaldoEstoqueDto(Guid SuprimentoId, decimal Quantidade, decimal CustoMedio);

public sealed record EntradaEstoqueDto(Guid Id, Guid? PedidoFornecedorId, DateTimeOffset CriadaEm,
    decimal TotalCusto, IReadOnlyList<ItemEntradaDto> Itens);
