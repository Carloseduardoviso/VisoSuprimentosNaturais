using VisoERP.Application.DTOs.Financeiro;

namespace VisoERP.Application.Interface.Financeiro;

public interface IFinanceiroAppService
{
    Task<Guid> RegistrarInvestimentoAsync(string descricao, decimal valor, DateOnly data, CancellationToken ct);
    Task<Guid> RegistrarDespesaAsync(string descricao, decimal valor, DateOnly competencia,
        bool paga, CancellationToken ct);
    Task PagarDespesaAsync(Guid id, CancellationToken ct);
    Task PagarContaAsync(Guid id, decimal valor, CancellationToken ct);
    Task<IReadOnlyList<InvestimentoDto>> ListarInvestimentosAsync(CancellationToken ct);
    Task<IReadOnlyList<DespesaDto>> ListarDespesasAsync(CancellationToken ct);
    Task<IReadOnlyList<ContaPagarDto>> ListarContasPagarAsync(CancellationToken ct);
    Task<ResumoFinanceiroDto> ResumirAsync(DateOnly inicio, DateOnly fim, CancellationToken ct);
}
