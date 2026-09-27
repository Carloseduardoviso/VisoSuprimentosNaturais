using System.ComponentModel.DataAnnotations;

namespace VisoERP.Web.Models.Comercial;

public sealed class VendaViewModel
{
    [Required(ErrorMessage = "Selecione o cliente.")] public Guid? ClienteId { get; set; }
    [Range(0, 999999999)] public decimal Desconto { get; set; }
    [Range(0, 999999999)] public decimal ValorEntrada { get; set; }
    [Range(0, 12)] public int NumeroParcelas { get; set; } = 1;
    public DateOnly PrimeiroVencimento { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddMonths(1));
    public List<ItemVendaViewModel> Itens { get; set; } = [];
}

public sealed class ItemVendaViewModel
{
    [Required] public Guid? SuprimentoId { get; set; }
    [Range(0.001, 999999999)] public decimal Quantidade { get; set; } = 1;
    [Range(0, 999999999)] public decimal PrecoCatalogo { get; set; }
    [Range(0, 999999999)] public decimal PrecoUnitario { get; set; }
    [Required] public DateTime Data { get; set; } = DateTime.Now;
    public bool Promocional { get; set; }
}
