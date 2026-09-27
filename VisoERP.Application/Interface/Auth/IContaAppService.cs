namespace VisoERP.Application.Interface.Auth;

public interface IContaAppService
{
    Task<ResultadoLogin> EntrarAsync(string email, string senha, bool lembrar, CancellationToken cancellationToken);
    Task SairAsync(CancellationToken cancellationToken);
}
