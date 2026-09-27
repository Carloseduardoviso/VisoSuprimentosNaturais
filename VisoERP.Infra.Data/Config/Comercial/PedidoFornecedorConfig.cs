using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisoERP.Domain.Entities.Comercial;
using VisoERP.Domain.Entities.Cadastros;

namespace VisoERP.Infra.Data.Config.Comercial;

public sealed class PedidoFornecedorConfig : IEntityTypeConfiguration<PedidoFornecedor>
{
    public void Configure(EntityTypeBuilder<PedidoFornecedor> builder)
    {
        builder.ToTable("PedidosFornecedores");
        builder.HasKey(x => x.Id);
        builder.HasOne<Fornecedor>().WithMany().HasForeignKey(x => x.FornecedorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Itens).WithOne().HasForeignKey(x => x.PedidoFornecedorId);
        builder.Navigation(x => x.Itens).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Ignore(x => x.Total);
        builder.HasIndex(x => new { x.FornecedorId, x.DataCriacao });
    }
}
