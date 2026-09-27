using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisoERP.Domain.Entities.Cadastros;
using VisoERP.Domain.Entities.Estoque;
using VisoERP.Domain.Entities.Financeiro;

namespace VisoERP.Infra.Data.Config.Financeiro;

public sealed class ContaPagarConfig : IEntityTypeConfiguration<ContaPagar>
{
    public void Configure(EntityTypeBuilder<ContaPagar> builder)
    {
        builder.ToTable("ContasPagar");
        builder.HasKey(x => x.Id);
        builder.HasOne<EntradaEstoque>().WithMany().HasForeignKey(x => x.EntradaEstoqueId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Fornecedor>().WithMany().HasForeignKey(x => x.FornecedorId).OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.Descricao).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Valor).HasPrecision(18, 2);
        builder.Property(x => x.ValorPago).HasPrecision(18, 2);
        builder.Ignore(x => x.Saldo);
        builder.HasIndex(x => x.EntradaEstoqueId).IsUnique();
        builder.HasIndex(x => x.Vencimento);
    }
}
