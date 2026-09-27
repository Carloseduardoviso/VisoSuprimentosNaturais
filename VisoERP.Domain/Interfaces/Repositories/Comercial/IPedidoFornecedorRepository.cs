using VisoERP.Domain.Entities.Comercial;

namespace VisoERP.Domain.Interfaces.Repositories.Comercial;

public interface IPedidoFornecedorRepository
{
    Task<PedidoFornecedor?> ObterComItensAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<PedidoFornecedor>> ListarComItensAsync(CancellationToken cancellationToken);
}
