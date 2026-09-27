using VisoERP.Domain.Entities.Cadastros;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using VisoERP.Application.DependencyInjection;
using VisoERP.Application.DTOs.Cadastros;

namespace VisoERP.Tests;

public class CadastrosTests
{
    [Fact]
    public void ClienteExigeNome()
    {
        Assert.Throws<ArgumentException>(() => Cliente.Criar(" ", null, null, null));
    }

    [Fact]
    public void FornecedorNormalizaNomeEDocumento()
    {
        var fornecedor = Fornecedor.Criar("  Ervas Norte  ", " 123 ", null, null);
        Assert.Equal("Ervas Norte", fornecedor.Nome);
        Assert.Equal("123", fornecedor.Documento);
    }

    [Fact]
    public void CategoriaPodeSerInativada()
    {
        var categoria = Categoria.Criar("Ervas");
        categoria.Atualizar("Ervas", false);
        Assert.False(categoria.Ativa);
    }

    [Fact]
    public void AutoMapperConverteOsTresCadastros()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.RegistrarApplication(null);
        using var provider = services.BuildServiceProvider();
        var mapper = provider.GetRequiredService<IMapper>();

        Assert.Equal("Ervas", mapper.Map<CategoriaDto>(Categoria.Criar("Ervas")).Nome);
        Assert.Equal("Ana", mapper.Map<ClienteDto>(Cliente.Criar("Ana", null, null, null)).Nome);
        Assert.Equal("Norte", mapper.Map<FornecedorDto>(Fornecedor.Criar("Norte", null, null, null)).Nome);
    }
}
