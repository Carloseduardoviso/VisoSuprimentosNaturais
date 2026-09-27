using Microsoft.Data.SqlClient;

namespace VisoERP.DatabaseMigrator;

public static class DatabasePrincipalProvisioner
{
    public static string QuoteIdentifier(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("SQL identifier must not be empty.", nameof(value));

        return $"[{value.Replace("]", "]]", StringComparison.Ordinal)}]";
    }

    public static string QuoteLiteral(string value) =>
        $"N'{value.Replace("'", "''", StringComparison.Ordinal)}'";

    public static async Task ProvisionAsync(
        SqlConnection openConnection,
        string databaseName,
        string loginName,
        string password,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(openConnection);
        if (openConnection.State != System.Data.ConnectionState.Open)
            throw new InvalidOperationException("SQL connection must be open before provisioning the application login.");
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Application database password must not be empty.", nameof(password));

        var login = QuoteIdentifier(loginName);
        var database = QuoteIdentifier(databaseName);
        var passwordLiteral = QuoteLiteral(password);
        var loginNameLiteral = QuoteLiteral(loginName);

        var commandText = $"""
            IF NOT EXISTS (SELECT 1 FROM sys.sql_logins WHERE name = {loginNameLiteral})
            BEGIN
                CREATE LOGIN {login} WITH PASSWORD = {passwordLiteral};
            END;

            USE {database};

            IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = {loginNameLiteral})
            BEGIN
                CREATE USER {login} FOR LOGIN {login};
            END;

            ALTER ROLE [db_datareader] ADD MEMBER {login};
            ALTER ROLE [db_datawriter] ADD MEMBER {login};
            """;

        await using var command = new SqlCommand(commandText, openConnection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
