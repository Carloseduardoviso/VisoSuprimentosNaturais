namespace VisoERP.Application.DTOs.Cadastros;

public sealed class SuprimentoDto
{
    public Guid Id { get; init; }
    public string CodigoInterno { get; init; } = string.Empty;
    public string Nome { get; init; } = string.Empty;
    public decimal PrecoCatalogo { get; init; }
    public decimal PrecoComDesconto { get; init; }
    public string? Descricao { get; init; }
    public string? FormaDeUso { get; init; }
    public string? ImagemCaminho { get; init; }
    public int QuantidadeMinimaCompra { get; init; }
    public int EstoqueMinimo { get; init; }
    public Guid? CategoriaId { get; init; }
    public string? CategoriaNome { get; init; }
    public bool Ativo { get; init; }
}
