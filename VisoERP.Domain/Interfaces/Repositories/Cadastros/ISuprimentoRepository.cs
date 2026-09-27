using VisoERP.Domain.Entities.Cadastros;

namespace VisoERP.Domain.Interfaces.Repositories.Cadastros;

public interface ISuprimentoRepository
{
    Task<Suprimento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Suprimento>> ListarAsync(CancellationToken cancellationToken);
    Task<bool> CodigoExisteAsync(string codigoInterno, Guid? ignorarId, CancellationToken cancellationToken);
    Task AdicionarAsync(Suprimento suprimento, CancellationToken cancellationToken);
}
