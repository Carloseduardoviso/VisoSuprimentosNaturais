using VisoERP.Application.DTOs.Comercial;

namespace VisoERP.Application.Interface.Comercial;

public interface IVendaAppService
{
    Task<Guid> RegistrarAsync(CriarVendaDto entrada, CancellationToken cancellationToken);
    Task RegistrarPagamentoAsync(Guid vendaId, Guid parcelaId, decimal valor,
        DateTimeOffset data, CancellationToken cancellationToken);
    Task<VendaDto?> ObterAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<VendaDto>> ListarAsync(CancellationToken cancellationToken);
}
