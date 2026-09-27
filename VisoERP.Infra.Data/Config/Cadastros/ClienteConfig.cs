using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisoERP.Domain.Entities.Cadastros;

namespace VisoERP.Infra.Data.Config.Cadastros;

public sealed class ClienteConfig : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Nome).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Documento).HasMaxLength(20);
        builder.Property(x => x.Email).HasMaxLength(256);
        builder.Property(x => x.Telefone).HasMaxLength(30);
        builder.Property(x => x.Cep).HasMaxLength(9);
        builder.Property(x => x.Numero).HasMaxLength(20);
        builder.Property(x => x.Endereco).HasMaxLength(300);
        builder.HasIndex(x => x.Nome);
        builder.HasIndex(x => x.Documento).IsUnique().HasFilter("[Documento] IS NOT NULL");
    }
}
