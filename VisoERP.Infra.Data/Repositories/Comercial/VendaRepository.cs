using Microsoft.EntityFrameworkCore;
using VisoERP.Domain.Entities.Comercial;
using VisoERP.Domain.Interfaces.Repositories.Comercial;
using VisoERP.Infra.Data.Context;

namespace VisoERP.Infra.Data.Repositories.Comercial;

public sealed class VendaRepository(VisoErpDbContext context) : IVendaRepository
{
    public Task<Venda?> ObterComItensEParcelasAsync(Guid id, CancellationToken cancellationToken) =>
        context.Set<Venda>().Include(x => x.Itens).Include(x => x.Parcelas)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Venda>> ListarComItensEParcelasAsync(CancellationToken cancellationToken) =>
        await context.Set<Venda>().AsNoTracking().AsSplitQuery()
            .Include(x => x.Itens).Include(x => x.Parcelas)
            .OrderByDescending(x => x.Data).ToListAsync(cancellationToken);
}
