using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisoERP.Domain.Entities.Cadastros;
using VisoERP.Domain.Entities.Estoque;

namespace VisoERP.Infra.Data.Config.Estoque;

public sealed class ItemEntradaEstoqueConfig : IEntityTypeConfiguration<ItemEntradaEstoque>
{
    public void Configure(EntityTypeBuilder<ItemEntradaEstoque> builder)
    {
        builder.ToTable("ItensEntradasEstoque");
        builder.HasKey(x => x.Id);
        builder.HasOne<Suprimento>().WithMany().HasForeignKey(x => x.SuprimentoId).OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.Quantidade).HasPrecision(18, 3);
        builder.Property(x => x.CustoUnitario).HasPrecision(18, 4);
        builder.Property(x => x.CodigoLote).HasMaxLength(80);
        builder.HasIndex(x => new { x.EntradaEstoqueId, x.SuprimentoId }).IsUnique();
    }
}
