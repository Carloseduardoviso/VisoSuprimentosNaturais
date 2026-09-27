namespace VisoERP.Application.DTOs.Comercial;

public sealed record CriarItemVendaDto(Guid SuprimentoId, decimal Quantidade, decimal PrecoCatalogo,
    decimal PrecoUnitario, bool Promocional, DateTimeOffset Data);

public sealed record CriarVendaDto(Guid ClienteId, decimal Desconto, decimal ValorEntrada,
    int NumeroParcelas, DateOnly PrimeiroVencimento, IReadOnlyList<CriarItemVendaDto> Itens);

public sealed record ItemVendaDto(Guid SuprimentoId, decimal Quantidade, decimal PrecoCatalogo,
    decimal PrecoUnitario, bool Promocional, DateTimeOffset Data, decimal CustoUnitarioHistorico,
    decimal Total, decimal CustoTotal);

public sealed record ParcelaVendaDto(Guid Id, int Numero, DateOnly Vencimento,
    decimal Valor, decimal ValorPago, decimal Saldo, DateTimeOffset? PagoEm);

public sealed record VendaDto(Guid Id, Guid ClienteId, DateTimeOffset Data, decimal Subtotal,
    decimal Desconto, decimal Total, decimal ValorEntrada, decimal ValorRecebido,
    decimal CustoTotal, IReadOnlyList<ItemVendaDto> Itens, IReadOnlyList<ParcelaVendaDto> Parcelas);
