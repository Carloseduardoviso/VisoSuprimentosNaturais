namespace VisoERP.Application.Interface.Auth;

public enum ResultadoLogin { Sucesso, Invalido, Bloqueado }

public interface IAuthGateway
{
    Task<ResultadoLogin> EntrarAsync(string email, string senha, bool lembrar, CancellationToken cancellationToken);
    Task SairAsync(CancellationToken cancellationToken);
}
