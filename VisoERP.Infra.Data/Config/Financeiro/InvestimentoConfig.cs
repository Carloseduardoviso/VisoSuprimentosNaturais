using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisoERP.Domain.Entities.Financeiro;

namespace VisoERP.Infra.Data.Config.Financeiro;

public sealed class InvestimentoConfig : IEntityTypeConfiguration<Investimento>
{
    public void Configure(EntityTypeBuilder<Investimento> builder)
    {
        builder.ToTable("Investimentos");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Descricao).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Valor).HasPrecision(18, 2);
        builder.HasIndex(x => x.Data);
    }
}
