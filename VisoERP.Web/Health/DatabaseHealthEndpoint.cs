using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VisoERP.Infra.Data.Context;

namespace VisoERP.Web.Health;

public interface IDatabaseConnectionProbe
{
    Task<bool> CanConnectAsync(CancellationToken cancellationToken);
}

public sealed class DatabaseConnectionProbe(VisoErpDbContext dbContext) : IDatabaseConnectionProbe
{
    public Task<bool> CanConnectAsync(CancellationToken cancellationToken) =>
        dbContext.Database.CanConnectAsync(cancellationToken);
}

public static class DatabaseHealthEndpoint
{
    public static IEndpointRouteBuilder MapDatabaseHealthCheck(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/health", async (IDatabaseConnectionProbe probe, CancellationToken cancellationToken) =>
        {
            try
            {
                return await probe.CanConnectAsync(cancellationToken)
                    ? Results.Text("healthy")
                    : Results.Text("unavailable", statusCode: StatusCodes.Status503ServiceUnavailable);
            }
            catch
            {
                return Results.Text("unavailable", statusCode: StatusCodes.Status503ServiceUnavailable);
            }
        }).AllowAnonymous();

        return endpoints;
    }
}
