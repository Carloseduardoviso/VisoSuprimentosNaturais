using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisoERP.Domain.Entities.Cadastros;
using VisoERP.Domain.Entities.Comercial;

namespace VisoERP.Infra.Data.Config.Comercial;

public sealed class VendaConfig : IEntityTypeConfiguration<Venda>
{
    public void Configure(EntityTypeBuilder<Venda> builder)
    {
        builder.ToTable("Vendas");
        builder.HasKey(x => x.Id);
        builder.HasOne<Cliente>().WithMany().HasForeignKey(x => x.ClienteId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Itens).WithOne().HasForeignKey(x => x.VendaId);
        builder.HasMany(x => x.Parcelas).WithOne().HasForeignKey(x => x.VendaId);
        builder.Navigation(x => x.Itens).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(x => x.Parcelas).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Property(x => x.Desconto).HasPrecision(18, 2);
        builder.Property(x => x.ValorEntrada).HasPrecision(18, 2);
        builder.Ignore(x => x.Subtotal);
        builder.Ignore(x => x.Total);
        builder.Ignore(x => x.CustoTotal);
        builder.Ignore(x => x.ValorRecebido);
        builder.HasIndex(x => new { x.ClienteId, x.Data });
    }
}
