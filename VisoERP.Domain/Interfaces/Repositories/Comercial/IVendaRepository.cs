using VisoERP.Domain.Entities.Comercial;

namespace VisoERP.Domain.Interfaces.Repositories.Comercial;

public interface IVendaRepository
{
    Task<Venda?> ObterComItensEParcelasAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Venda>> ListarComItensEParcelasAsync(CancellationToken cancellationToken);
}
