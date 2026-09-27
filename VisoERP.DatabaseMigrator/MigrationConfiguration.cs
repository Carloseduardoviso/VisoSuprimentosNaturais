using Microsoft.Data.SqlClient;

namespace VisoERP.DatabaseMigrator;

public sealed record MigrationConfiguration(
    string MigrationConnectionString,
    string DatabaseName,
    string AppLoginName,
    string AppPassword)
{
    public const string AppLogin = "visoerp_app";

    public static MigrationConfiguration Load(Func<string, string?> getEnvironmentVariable)
    {
        ArgumentNullException.ThrowIfNull(getEnvironmentVariable);

        var migrationConnectionString = getEnvironmentVariable("MIGRATION_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(migrationConnectionString))
            throw new InvalidOperationException("MIGRATION_CONNECTION_STRING must be configured.");

        var appPassword = getEnvironmentVariable("APP_DB_PASSWORD");
        if (string.IsNullOrWhiteSpace(appPassword))
            throw new InvalidOperationException("APP_DB_PASSWORD must be configured.");

        var databaseName = new SqlConnectionStringBuilder(migrationConnectionString).InitialCatalog;
        if (string.IsNullOrWhiteSpace(databaseName))
            throw new InvalidOperationException("MIGRATION_CONNECTION_STRING must specify a database.");

        return new MigrationConfiguration(migrationConnectionString, databaseName, AppLogin, appPassword);
    }
}
