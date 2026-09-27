using Microsoft.EntityFrameworkCore;
using VisoERP.Domain.Entities;
using VisoERP.Domain.Interfaces.Repositories;
using VisoERP.Infra.Data.Context;

namespace VisoERP.Infra.Data.Repositories;

public sealed class Repository<TEntity>(VisoErpDbContext context) : IRepository<TEntity>
    where TEntity : EntidadeBase
{
    public Task<TEntity?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<TEntity>> ListarAsync(CancellationToken cancellationToken = default) =>
        await context.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken);

    public Task AdicionarAsync(TEntity entidade, CancellationToken cancellationToken = default) =>
        context.Set<TEntity>().AddAsync(entidade, cancellationToken).AsTask();

    public void Atualizar(TEntity entidade) => context.Set<TEntity>().Update(entidade);
}
