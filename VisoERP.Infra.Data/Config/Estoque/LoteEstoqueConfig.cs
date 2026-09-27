using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisoERP.Domain.Entities.Cadastros;
using VisoERP.Domain.Entities.Estoque;

namespace VisoERP.Infra.Data.Config.Estoque;

public sealed class LoteEstoqueConfig : IEntityTypeConfiguration<LoteEstoque>
{
    public void Configure(EntityTypeBuilder<LoteEstoque> builder)
    {
        builder.ToTable("LotesEstoque");
        builder.HasKey(x => x.Id);
        builder.HasOne<Suprimento>().WithMany().HasForeignKey(x => x.SuprimentoId).OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.Codigo).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Quantidade).HasPrecision(18, 3);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasIndex(x => new { x.SuprimentoId, x.Codigo }).IsUnique();
        builder.HasIndex(x => x.Validade);
    }
}
