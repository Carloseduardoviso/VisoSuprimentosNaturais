using Microsoft.EntityFrameworkCore;
using VisoERP.Domain.Entities.Comercial;
using VisoERP.Domain.Interfaces.Repositories.Comercial;
using VisoERP.Infra.Data.Context;

namespace VisoERP.Infra.Data.Repositories.Comercial;

public sealed class PedidoFornecedorRepository(VisoErpDbContext context) : IPedidoFornecedorRepository
{
    public Task<PedidoFornecedor?> ObterComItensAsync(Guid id, CancellationToken cancellationToken) =>
        context.PedidosFornecedores.Include(x => x.Itens).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<PedidoFornecedor>> ListarComItensAsync(CancellationToken cancellationToken) =>
        await context.PedidosFornecedores.AsNoTracking().Include(x => x.Itens)
            .OrderByDescending(x => x.DataCriacao).ToListAsync(cancellationToken);

    public async Task<bool> AtualizarDataAsync(Guid id, DateTimeOffset dataPedido, CancellationToken cancellationToken) =>
        await context.PedidosFornecedores
            .Where(x => x.Id == id && x.Situacao == VisoERP.Domain.Enums.SituacaoPedido.Pendente)
            .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.DataCriacao, dataPedido), cancellationToken) == 1;
}
