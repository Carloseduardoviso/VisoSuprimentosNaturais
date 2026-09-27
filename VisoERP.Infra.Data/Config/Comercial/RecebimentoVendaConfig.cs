using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisoERP.Domain.Entities.Comercial;

namespace VisoERP.Infra.Data.Config.Comercial;

public sealed class RecebimentoVendaConfig : IEntityTypeConfiguration<RecebimentoVenda>
{
    public void Configure(EntityTypeBuilder<RecebimentoVenda> builder)
    {
        builder.ToTable("RecebimentosVendas");
        builder.HasKey(x => x.Id);
        builder.HasOne<Venda>().WithMany().HasForeignKey(x => x.VendaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ParcelaVenda>().WithMany().HasForeignKey(x => x.ParcelaVendaId).OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.Valor).HasPrecision(18, 2);
        builder.HasIndex(x => x.Data);
    }
}
