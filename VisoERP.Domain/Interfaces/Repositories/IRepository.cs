using VisoERP.Domain.Entities;

namespace VisoERP.Domain.Interfaces.Repositories;

public interface IRepository<TEntity> where TEntity : EntidadeBase
{
    Task<TEntity?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> ListarAsync(CancellationToken cancellationToken = default);
    Task AdicionarAsync(TEntity entidade, CancellationToken cancellationToken = default);
    void Atualizar(TEntity entidade);
}
