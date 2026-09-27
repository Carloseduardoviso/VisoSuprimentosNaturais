using VisoERP.Application.DTOs.Cadastros;

namespace VisoERP.Application.Interface.Cadastros;

public interface ISuprimentoAppService
{
    Task<IReadOnlyList<SuprimentoDto>> ListarAsync(CancellationToken cancellationToken);
    Task<SuprimentoDto?> ObterAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<CategoriaDto>> ListarCategoriasAsync(CancellationToken cancellationToken);
    Task<Guid> SalvarAsync(Guid? id, SalvarSuprimentoDto entrada, CancellationToken cancellationToken);
}
