using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisoERP.Domain.Entities.Cadastros;
using VisoERP.Domain.Entities.Estoque;

namespace VisoERP.Infra.Data.Config.Estoque;

public sealed class EstoqueProdutoConfig : IEntityTypeConfiguration<EstoqueProduto>
{
    public void Configure(EntityTypeBuilder<EstoqueProduto> builder)
    {
        builder.ToTable("EstoquesProdutos");
        builder.HasKey(x => x.Id);
        builder.HasOne<Suprimento>().WithMany().HasForeignKey(x => x.SuprimentoId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.SuprimentoId).IsUnique();
        builder.Property(x => x.Quantidade).HasPrecision(18, 3);
        builder.Property(x => x.CustoMedio).HasPrecision(18, 4);
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
