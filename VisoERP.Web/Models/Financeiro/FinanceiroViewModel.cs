using System.ComponentModel.DataAnnotations;
using VisoERP.Application.DTOs.Financeiro;

namespace VisoERP.Web.Models.Financeiro;

public sealed class FinanceiroViewModel
{
    public DateOnly Inicio { get; set; }
    public DateOnly Fim { get; set; }
    public ResumoFinanceiroDto? Resumo { get; set; }
    public IReadOnlyList<InvestimentoDto> Investimentos { get; set; } = [];
    public IReadOnlyList<DespesaDto> Despesas { get; set; } = [];
    public IReadOnlyList<ContaPagarDto> ContasPagar { get; set; } = [];
}

public sealed class RegistroFinanceiroViewModel
{
    [Required, StringLength(200)] public string Descricao { get; set; } = string.Empty;
    [Range(0.01, 999999999)] public decimal Valor { get; set; }
    [Required] public DateOnly Data { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public bool Paga { get; set; }
}
