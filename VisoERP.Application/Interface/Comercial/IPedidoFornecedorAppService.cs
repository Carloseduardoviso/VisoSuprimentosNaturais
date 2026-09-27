using VisoERP.Application.DTOs.Comercial;

namespace VisoERP.Application.Interface.Comercial;

public interface IPedidoFornecedorAppService
{
    Task<Guid> CriarAsync(CriarPedidoDto pedido, CancellationToken cancellationToken);
    Task<IReadOnlyList<PedidoFornecedorDto>> ListarAsync(CancellationToken cancellationToken);
    Task<PedidoFornecedorDto?> ObterAsync(Guid id, CancellationToken cancellationToken);
    Task CancelarAsync(Guid id, CancellationToken cancellationToken);
    Task RegistrarFaltaAsync(Guid id, Guid suplementoId, decimal quantidade, CancellationToken cancellationToken);
}
