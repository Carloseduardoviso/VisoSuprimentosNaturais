using VisoERP.Domain.Entities.Cadastros;

namespace VisoERP.Tests;

public class SuprimentoTests
{
    [Fact]
    public void DescontoNaoPodeSuperarPrecoCatalogo()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Suprimento.Criar("CAM-001", "Camomila", 10m, 11m, 1));
    }

    [Fact]
    public void CompraMinimaDeveSerPositiva()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Suprimento.Criar("CAM-001", "Camomila", 10m, 9m, 0));
    }
}
