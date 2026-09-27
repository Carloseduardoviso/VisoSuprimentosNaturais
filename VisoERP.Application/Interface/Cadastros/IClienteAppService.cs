using VisoERP.Application.DTOs.Cadastros;

namespace VisoERP.Application.Interface.Cadastros;

public interface IClienteAppService
{
    Task<IReadOnlyList<ClienteDto>> ListarAsync(CancellationToken cancellationToken);
    Task<ClienteDto?> ObterAsync(Guid id, CancellationToken cancellationToken);
    Task<Guid> SalvarAsync(Guid? id, SalvarClienteDto entrada, CancellationToken cancellationToken);
}
