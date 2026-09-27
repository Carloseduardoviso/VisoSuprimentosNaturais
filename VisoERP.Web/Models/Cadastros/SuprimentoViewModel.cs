using System.ComponentModel.DataAnnotations;

namespace VisoERP.Web.Models.Cadastros;

public sealed class SuprimentoViewModel
{
    public Guid? Id { get; set; }
    [Required, StringLength(50)]
    public string CodigoInterno { get; set; } = string.Empty;
    [Required, StringLength(200)]
    public string Nome { get; set; } = string.Empty;
    [Range(0, 9999999999999999.99)]
    public decimal PrecoCatalogo { get; set; }
    [Range(0, 9999999999999999.99)]
    public decimal PrecoComDesconto { get; set; }
    [StringLength(2000)]
    public string? Descricao { get; set; }
    [StringLength(2000)]
    public string? FormaDeUso { get; set; }
    [Range(1, int.MaxValue)]
    public int QuantidadeMinimaCompra { get; set; } = 1;
    [Range(0, int.MaxValue)]
    public int EstoqueMinimo { get; set; }
    public Guid? CategoriaId { get; set; }
    public bool Ativo { get; set; } = true;
    public IFormFile? Imagem { get; set; }
}
