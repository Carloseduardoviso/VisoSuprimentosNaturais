using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisoERP.Domain.Entities.Cadastros;

namespace VisoERP.Infra.Data.Config.Cadastros;

public sealed class CategoriaConfig : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("Categorias");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Nome).HasMaxLength(120).IsRequired();
        builder.HasIndex(x => x.Nome).IsUnique();
    }
}
