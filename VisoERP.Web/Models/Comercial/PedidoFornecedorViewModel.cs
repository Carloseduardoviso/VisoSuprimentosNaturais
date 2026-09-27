using System.ComponentModel.DataAnnotations;

namespace VisoERP.Web.Models.Comercial;

public sealed class PedidoFornecedorViewModel
{
    [Required(ErrorMessage = "Selecione o fornecedor.")]
    public Guid? FornecedorId { get; set; }
    public List<ItemPedidoViewModel> Itens { get; set; } = [];
}

public sealed class ItemPedidoViewModel
{
    [Required] public Guid? SuprimentoId { get; set; }
    [Range(0.001, 999999999)] public decimal Quantidade { get; set; } = 1;
    [Range(0, 999999999)] public decimal PrecoCatalogo { get; set; }
    [Range(0, 999999999)] public decimal PrecoComDesconto { get; set; }
}
