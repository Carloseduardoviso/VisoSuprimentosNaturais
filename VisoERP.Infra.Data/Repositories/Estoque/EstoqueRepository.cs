using Microsoft.EntityFrameworkCore;
using VisoERP.Domain.Entities.Estoque;
using VisoERP.Domain.Interfaces.Repositories.Estoque;
using VisoERP.Infra.Data.Context;

namespace VisoERP.Infra.Data.Repositories.Estoque;

public sealed class EstoqueRepository(VisoErpDbContext context) : IEstoqueRepository
{
    public Task<EstoqueProduto?> ObterSaldoAsync(Guid suprimentoId, CancellationToken cancellationToken) =>
        context.EstoquesProdutos.SingleOrDefaultAsync(x => x.SuprimentoId == suprimentoId, cancellationToken);

    public Task<LoteEstoque?> ObterLoteAsync(Guid suprimentoId, string codigo, CancellationToken cancellationToken) =>
        context.LotesEstoque.SingleOrDefaultAsync(x => x.SuprimentoId == suprimentoId && x.Codigo == codigo, cancellationToken);

    public async Task<IReadOnlyList<EstoqueProduto>> ListarSaldosAsync(CancellationToken cancellationToken) =>
        await context.EstoquesProdutos.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<EntradaEstoque>> ListarEntradasComItensAsync(CancellationToken cancellationToken) =>
        await context.EntradasEstoque.AsNoTracking().Include(x => x.Itens)
            .OrderByDescending(x => x.CriadaEm).ToListAsync(cancellationToken);
}
