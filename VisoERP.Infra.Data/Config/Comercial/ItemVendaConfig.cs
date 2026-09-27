using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisoERP.Domain.Entities.Cadastros;
using VisoERP.Domain.Entities.Comercial;

namespace VisoERP.Infra.Data.Config.Comercial;

public sealed class ItemVendaConfig : IEntityTypeConfiguration<ItemVenda>
{
    public void Configure(EntityTypeBuilder<ItemVenda> builder)
    {
        builder.ToTable("ItensVendas");
        builder.HasKey(x => x.Id);
        builder.HasOne<Suprimento>().WithMany().HasForeignKey(x => x.SuprimentoId).OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.Quantidade).HasPrecision(18, 3);
        builder.Property(x => x.PrecoCatalogo).HasPrecision(18, 2);
        builder.Property(x => x.PrecoUnitario).HasPrecision(18, 2);
        builder.Property(x => x.CustoUnitarioHistorico).HasPrecision(18, 4);
        builder.Ignore(x => x.Total);
        builder.Ignore(x => x.CustoTotal);
        builder.HasIndex(x => new { x.VendaId, x.SuprimentoId }).IsUnique();
    }
}
