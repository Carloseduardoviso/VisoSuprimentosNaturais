using VisoERP.Application.DTOs.Cadastros;

namespace VisoERP.Application.Interface.Cadastros;

public interface ICategoriaAppService
{
    Task<IReadOnlyList<CategoriaDto>> ListarAsync(CancellationToken cancellationToken);
    Task<CategoriaDto?> ObterAsync(Guid id, CancellationToken cancellationToken);
    Task<Guid> SalvarAsync(Guid? id, SalvarCategoriaDto entrada, CancellationToken cancellationToken);
}
