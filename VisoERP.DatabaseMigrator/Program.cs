using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using VisoERP.Infra.Data.Context;

namespace VisoERP.DatabaseMigrator;

public static class Program
{
    public static async Task<int> Main()
    {
        try
        {
            var configuration = MigrationConfiguration.Load(Environment.GetEnvironmentVariable);
            var options = new DbContextOptionsBuilder<VisoErpDbContext>()
                .UseSqlServer(configuration.MigrationConnectionString)
                .Options;

            await using (var dbContext = new VisoErpDbContext(options))
            {
                await dbContext.Database.MigrateAsync();
            }

            await using var connection = new SqlConnection(configuration.MigrationConnectionString);
            await connection.OpenAsync();
            await DatabasePrincipalProvisioner.ProvisionAsync(
                connection,
                configuration.DatabaseName,
                configuration.AppLoginName,
                configuration.AppPassword,
                CancellationToken.None);

            Console.WriteLine("Database migrations and application login provisioning completed.");
            return 0;
        }
        catch
        {
            Console.Error.WriteLine("Database migration failed. Check the protected server configuration and service logs.");
            return 1;
        }
    }
}
