using Microsoft.EntityFrameworkCore;
using VisoERP.Domain.Entities.Cadastros;
using VisoERP.Domain.Interfaces.Repositories.Cadastros;
using VisoERP.Infra.Data.Context;

namespace VisoERP.Infra.Data.Repositories.Cadastros;

public sealed class SuprimentoRepository(VisoErpDbContext context) : ISuprimentoRepository
{
    public Task<Suprimento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        context.Suprimentos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Suprimento>> ListarAsync(CancellationToken cancellationToken) =>
        await context.Suprimentos.AsNoTracking().Include(x => x.Categoria).OrderBy(x => x.Nome).ToListAsync(cancellationToken);

    public Task<bool> CodigoExisteAsync(string codigoInterno, Guid? ignorarId, CancellationToken cancellationToken) =>
        context.Suprimentos.AnyAsync(x => x.CodigoInterno == codigoInterno && x.Id != ignorarId,
            cancellationToken);

    public Task AdicionarAsync(Suprimento suprimento, CancellationToken cancellationToken) =>
        context.Suprimentos.AddAsync(suprimento, cancellationToken).AsTask();
}
