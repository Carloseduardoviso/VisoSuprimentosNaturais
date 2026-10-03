using Microsoft.EntityFrameworkCore;
using VisoERP.Domain.Entities.Comercial;
using VisoERP.Domain.Interfaces.Repositories.Comercial;
using VisoERP.Infra.Data.Context;

namespace VisoERP.Infra.Data.Repositories.Comercial;

public sealed class VendaRepository(VisoErpDbContext context) : IVendaRepository
{
    public Task<Venda?> ObterComItensEParcelasAsync(Guid id, CancellationToken cancellationToken) =>
        context.Set<Venda>().Include(x => x.Itens).Include(x => x.Parcelas)
            .Include(x => x.Recebimentos)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Venda>> ListarComItensEParcelasAsync(CancellationToken cancellationToken) =>
        await context.Set<Venda>().AsNoTracking().AsSplitQuery()
            .Include(x => x.Itens).Include(x => x.Parcelas).Include(x => x.Recebimentos)
            .OrderByDescending(x => x.Data).ToListAsync(cancellationToken);

    public void Excluir(Venda venda)
    {
        context.Set<RecebimentoVenda>().RemoveRange(venda.Recebimentos);
        context.Set<ParcelaVenda>().RemoveRange(venda.Parcelas);
        context.Set<ItemVenda>().RemoveRange(venda.Itens);
        context.Set<Venda>().Remove(venda);
    }

    public async Task PrepararAtualizacaoDeRascunhoAsync(Guid vendaId, CancellationToken cancellationToken)
    {
        await context.Set<RecebimentoVenda>()
            .Where(x => x.VendaId == vendaId)
            .ExecuteDeleteAsync(cancellationToken);
        await context.Set<ParcelaVenda>()
            .Where(x => x.VendaId == vendaId)
            .ExecuteDeleteAsync(cancellationToken);
        await context.Set<ItemVenda>()
            .Where(x => x.VendaId == vendaId)
            .ExecuteDeleteAsync(cancellationToken);
        foreach (var entry in context.ChangeTracker.Entries<RecebimentoVenda>()
                     .Where(x => x.Entity.VendaId == vendaId).ToList())
            entry.State = EntityState.Detached;
        foreach (var entry in context.ChangeTracker.Entries<ParcelaVenda>()
                     .Where(x => x.Entity.VendaId == vendaId).ToList())
            entry.State = EntityState.Detached;
        foreach (var entry in context.ChangeTracker.Entries<ItemVenda>()
                     .Where(x => x.Entity.VendaId == vendaId).ToList())
            entry.State = EntityState.Detached;
    }

    public async Task AtualizarRascunhoAsync(Venda venda, CancellationToken cancellationToken)
    {
        context.ChangeTracker.Clear();
        var afetadas = await context.Set<Venda>()
            .Where(x => x.Id == venda.Id && !x.Finalizada)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.ClienteId, venda.ClienteId)
                .SetProperty(x => x.Desconto, venda.Desconto)
                .SetProperty(x => x.ValorEntrada, venda.ValorEntrada)
                .SetProperty(x => x.NumeroParcelas, venda.NumeroParcelas)
                .SetProperty(x => x.PrimeiroVencimento, venda.PrimeiroVencimento), cancellationToken);
        if (afetadas != 1)
            throw new DbUpdateConcurrencyException("O rascunho não existe mais ou já foi finalizado.");

        await context.Set<RecebimentoVenda>()
            .Where(x => x.VendaId == venda.Id).ExecuteDeleteAsync(cancellationToken);
        await context.Set<ParcelaVenda>()
            .Where(x => x.VendaId == venda.Id).ExecuteDeleteAsync(cancellationToken);
        await context.Set<ItemVenda>()
            .Where(x => x.VendaId == venda.Id).ExecuteDeleteAsync(cancellationToken);
        await context.Set<ItemVenda>().AddRangeAsync(venda.Itens, cancellationToken);
    }
}
