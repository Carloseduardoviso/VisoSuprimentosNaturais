using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisoERP.Domain.Entities.Financeiro;

namespace VisoERP.Infra.Data.Config.Financeiro;

public sealed class PagamentoContaPagarConfig : IEntityTypeConfiguration<PagamentoContaPagar>
{
    public void Configure(EntityTypeBuilder<PagamentoContaPagar> builder)
    {
        builder.ToTable("PagamentosContasPagar");
        builder.HasKey(x => x.Id);
        builder.HasOne<ContaPagar>().WithMany().HasForeignKey(x => x.ContaPagarId).OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.Valor).HasPrecision(18, 2);
        builder.HasIndex(x => x.Data);
    }
}
