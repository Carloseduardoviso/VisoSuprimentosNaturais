namespace VisoERP.Application.DTOs.Cadastros;

public sealed record SalvarSuprimentoDto(
    string CodigoInterno, string Nome, decimal PrecoCatalogo, decimal PrecoComDesconto,
    int QuantidadeMinimaCompra, int EstoqueMinimo, Guid? CategoriaId,
    string? Descricao, string? FormaDeUso, string? ImagemCaminho, bool Ativo);
