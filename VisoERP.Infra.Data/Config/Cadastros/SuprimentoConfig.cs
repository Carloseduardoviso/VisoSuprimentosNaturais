using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisoERP.Domain.Entities.Cadastros;

namespace VisoERP.Infra.Data.Config.Cadastros;

public sealed class SuprimentoConfig : IEntityTypeConfiguration<Suprimento>
{
    public void Configure(EntityTypeBuilder<Suprimento> builder)
    {
        builder.ToTable("Suprimentos");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CodigoInterno).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.CodigoInterno).IsUnique();
        builder.Property(x => x.Nome).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.Nome);
        builder.Property(x => x.PrecoCatalogo).HasPrecision(18, 2);
        builder.Property(x => x.PrecoComDesconto).HasPrecision(18, 2);
        builder.Property(x => x.Descricao).HasMaxLength(2000);
        builder.Property(x => x.FormaDeUso).HasMaxLength(2000);
        builder.Property(x => x.ImagemCaminho).HasMaxLength(500);
        builder.HasOne(x => x.Categoria).WithMany()
            .HasForeignKey(x => x.CategoriaId).OnDelete(DeleteBehavior.Restrict);
    }
}
