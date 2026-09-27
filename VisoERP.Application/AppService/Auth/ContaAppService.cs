using VisoERP.Application.Interface.Auth;

namespace VisoERP.Application.AppService.Auth;

public sealed class ContaAppService(IAuthGateway gateway) : IContaAppService
{
    public Task<ResultadoLogin> EntrarAsync(string cpf, string senha, bool lembrar, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(cpf) || string.IsNullOrWhiteSpace(senha))
            return Task.FromResult(ResultadoLogin.Invalido);
        return gateway.EntrarAsync(cpf.Trim(), senha, lembrar, cancellationToken);
    }

    public Task SairAsync(CancellationToken cancellationToken) => gateway.SairAsync(cancellationToken);
}
