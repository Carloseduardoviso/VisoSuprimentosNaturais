using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisoERP.Domain.Entities.Comercial;
using VisoERP.Domain.Entities.Estoque;

namespace VisoERP.Infra.Data.Config.Estoque;

public sealed class EntradaEstoqueConfig : IEntityTypeConfiguration<EntradaEstoque>
{
    public void Configure(EntityTypeBuilder<EntradaEstoque> builder)
    {
        builder.ToTable("EntradasEstoque");
        builder.HasKey(x => x.Id);
        builder.HasOne<PedidoFornecedor>().WithMany().HasForeignKey(x => x.PedidoFornecedorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Itens).WithOne().HasForeignKey(x => x.EntradaEstoqueId);
        builder.Navigation(x => x.Itens).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Ignore(x => x.TotalCusto);
        builder.HasIndex(x => x.CriadaEm);
    }
}
