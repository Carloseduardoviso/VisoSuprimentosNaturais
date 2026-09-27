using Microsoft.EntityFrameworkCore;
using VisoERP.Domain.Entities.Cadastros;
using VisoERP.Infra.Data.Context;

namespace VisoERP.Tests;

public class CategoriaTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NomeVazioEhRejeitado(string nome) =>
        Assert.Throws<ArgumentException>(() => Categoria.Criar(nome));

    [Fact]
    public void NomeEhNormalizado()
    {
        var categoria = Categoria.Criar("  Ervas  ");
        Assert.Equal("Ervas", categoria.Nome);
        Assert.True(categoria.Ativa);
        Assert.NotEqual(Guid.Empty, categoria.Id);
    }

    [Fact]
    public void MapeamentoDefineNomeUnico()
    {
        var options = new DbContextOptionsBuilder<VisoErpDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=VisoERP_TesteModelo;Trusted_Connection=True")
            .Options;
        using var context = new VisoErpDbContext(options);
        var categoria = context.Model.FindEntityType(typeof(Categoria));
        Assert.NotNull(categoria);
        Assert.Contains(categoria.GetIndexes(), index => index.IsUnique &&
            index.Properties.Count == 1 && index.Properties[0].Name == nameof(Categoria.Nome));
    }
}
