using VisoERP.Domain.Interfaces.Repositories;
using VisoERP.Infra.Data.Context;

namespace VisoERP.Infra.Data.UnitOfWork;

public sealed class UnitOfWork(VisoErpDbContext context) : IUnitOfWork
{
    public Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);

    public async Task ExecutarEmTransacaoAsync(Func<CancellationToken, Task> operacao,
        CancellationToken cancellationToken = default)
    {
        await using var transacao = await context.Database.BeginTransactionAsync(cancellationToken);
        await operacao(cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await transacao.CommitAsync(cancellationToken);
    }
}
