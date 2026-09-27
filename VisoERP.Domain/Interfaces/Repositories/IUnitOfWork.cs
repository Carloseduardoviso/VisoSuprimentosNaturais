namespace VisoERP.Domain.Interfaces.Repositories;

public interface IUnitOfWork
{
    Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
    Task ExecutarEmTransacaoAsync(Func<CancellationToken, Task> operacao,
        CancellationToken cancellationToken = default);
}
