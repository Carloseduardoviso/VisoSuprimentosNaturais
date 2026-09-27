using System.ComponentModel.DataAnnotations;

namespace VisoERP.Web.Models.Estoque;

public sealed class EntradaEstoqueViewModel
{
    public Guid? PedidoFornecedorId { get; set; }
    public List<ItemEntradaViewModel> Itens { get; set; } = [];
}

public sealed class ItemEntradaViewModel
{
    [Required] public Guid? SuprimentoId { get; set; }
    [Range(0.001, 999999999)] public decimal Quantidade { get; set; } = 1;
    [Range(0, 999999999)] public decimal CustoUnitario { get; set; }
    [Required] public DateTime Data { get; set; } = DateTime.Now;
    public bool Promocional { get; set; }
    [StringLength(80)] public string? CodigoLote { get; set; }
    public DateOnly? Validade { get; set; }
}
