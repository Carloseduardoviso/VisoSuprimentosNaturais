using VisoERP.Application.AppService.Auth;
using VisoERP.Application.Interface.Auth;

namespace VisoERP.Tests;

public class ContaAppServiceTests
{
    [Fact]
    public async Task EmailEmBrancoNaoAcionaAutenticacao()
    {
        var gateway = new GatewayDeTeste();
        var service = new ContaAppService(gateway);

        var resultado = await service.EntrarAsync("  ", "senha", false, CancellationToken.None);

        Assert.Equal(ResultadoLogin.Invalido, resultado);
        Assert.False(gateway.FoiChamado);
    }

    private sealed class GatewayDeTeste : IAuthGateway
    {
        public bool FoiChamado { get; private set; }
        public Task<ResultadoLogin> EntrarAsync(string email, string senha, bool lembrar,
            CancellationToken cancellationToken)
        {
            FoiChamado = true;
            return Task.FromResult(ResultadoLogin.Sucesso);
        }
        public Task SairAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
