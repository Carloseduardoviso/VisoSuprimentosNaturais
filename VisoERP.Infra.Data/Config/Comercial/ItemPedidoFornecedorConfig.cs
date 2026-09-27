using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisoERP.Domain.Entities.Comercial;
using VisoERP.Domain.Entities.Cadastros;

namespace VisoERP.Infra.Data.Config.Comercial;

public sealed class ItemPedidoFornecedorConfig : IEntityTypeConfiguration<ItemPedidoFornecedor>
{
    public void Configure(EntityTypeBuilder<ItemPedidoFornecedor> builder)
    {
        builder.ToTable("ItensPedidosFornecedores");
        builder.HasKey(x => x.Id);
        builder.HasOne<Suprimento>().WithMany().HasForeignKey(x => x.SuprimentoId).OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.Quantidade).HasPrecision(18, 3);
        builder.Property(x => x.QuantidadeRecebida).HasPrecision(18, 3);
        builder.Property(x => x.QuantidadeNaoRecebida).HasPrecision(18, 3);
        builder.Property(x => x.PrecoCatalogo).HasPrecision(18, 2);
        builder.Property(x => x.PrecoComDesconto).HasPrecision(18, 2);
        builder.Ignore(x => x.Total);
        builder.HasIndex(x => new { x.PedidoFornecedorId, x.SuprimentoId }).IsUnique();
    }
}
