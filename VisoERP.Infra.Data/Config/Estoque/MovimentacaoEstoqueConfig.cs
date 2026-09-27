using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisoERP.Domain.Entities.Cadastros;
using VisoERP.Domain.Entities.Estoque;

namespace VisoERP.Infra.Data.Config.Estoque;

public sealed class MovimentacaoEstoqueConfig : IEntityTypeConfiguration<MovimentacaoEstoque>
{
    public void Configure(EntityTypeBuilder<MovimentacaoEstoque> builder)
    {
        builder.ToTable("MovimentacoesEstoque");
        builder.HasKey(x => x.Id);
        builder.HasOne<Suprimento>().WithMany().HasForeignKey(x => x.SuprimentoId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<LoteEstoque>().WithMany().HasForeignKey(x => x.LoteEstoqueId).OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.Quantidade).HasPrecision(18, 3);
        builder.Property(x => x.CustoUnitario).HasPrecision(18, 4);
        builder.HasIndex(x => new { x.SuprimentoId, x.Data });
        builder.HasIndex(x => x.ReferenciaId);
    }
}
