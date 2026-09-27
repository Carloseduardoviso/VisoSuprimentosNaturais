using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisoERP.Domain.Entities.Comercial;

namespace VisoERP.Infra.Data.Config.Comercial;

public sealed class ParcelaVendaConfig : IEntityTypeConfiguration<ParcelaVenda>
{
    public void Configure(EntityTypeBuilder<ParcelaVenda> builder)
    {
        builder.ToTable("ParcelasVendas");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Valor).HasPrecision(18, 2);
        builder.Property(x => x.ValorPago).HasPrecision(18, 2);
        builder.Ignore(x => x.Saldo);
        builder.HasIndex(x => new { x.VendaId, x.Numero }).IsUnique();
        builder.HasIndex(x => x.Vencimento);
    }
}
