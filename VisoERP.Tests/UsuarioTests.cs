using VisoERP.Domain.Entities;

namespace VisoERP.Tests;

public sealed class UsuarioTests
{
    [Fact]
    public void NormalizaCpfRemovendoPontuacao()
    {
        Assert.Equal("12345678909", Usuario.NormalizarCpf("123.456.789-09"));
    }
}
