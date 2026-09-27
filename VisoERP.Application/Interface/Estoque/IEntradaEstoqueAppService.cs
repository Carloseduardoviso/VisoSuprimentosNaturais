using VisoERP.Application.DTOs.Estoque;

namespace VisoERP.Application.Interface.Estoque;

public interface IEntradaEstoqueAppService
{
    Task<Guid> ConfirmarAsync(CriarEntradaDto entrada, CancellationToken cancellationToken);
    Task<IReadOnlyList<EntradaEstoqueDto>> ListarAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<SaldoEstoqueDto>> ListarSaldosAsync(CancellationToken cancellationToken);
}
