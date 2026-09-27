using VisoERP.Application.DTOs.Cadastros;

namespace VisoERP.Application.Interface.Cadastros;

public interface IFornecedorAppService
{
    Task<IReadOnlyList<FornecedorDto>> ListarAsync(CancellationToken cancellationToken);
    Task<FornecedorDto?> ObterAsync(Guid id, CancellationToken cancellationToken);
    Task<Guid> SalvarAsync(Guid? id, SalvarFornecedorDto entrada, CancellationToken cancellationToken);
}
