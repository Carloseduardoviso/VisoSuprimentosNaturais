using VisoERP.Domain.Entities.Estoque;

namespace VisoERP.Domain.Interfaces.Repositories.Estoque;

public interface IEstoqueRepository
{
    Task<EstoqueProduto?> ObterSaldoAsync(Guid suprimentoId, CancellationToken cancellationToken);
    Task<LoteEstoque?> ObterLoteAsync(Guid suprimentoId, string codigo, CancellationToken cancellationToken);
    Task<IReadOnlyList<EstoqueProduto>> ListarSaldosAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<EntradaEstoque>> ListarEntradasComItensAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<LoteEstoque>> ListarLotesDisponiveisAsync(Guid suprimentoId, CancellationToken cancellationToken);
}
