using VisoERP.DatabaseMigrator;

namespace VisoERP.Tests.Operations;

public sealed class DatabasePrincipalProvisionerTests
{
    [Theory]
    [InlineData("visoerp_app", "[visoerp_app]")]
    [InlineData("viso]erp", "[viso]]erp]")]
    public void QuoteIdentifier_wraps_and_escapes_sql_identifiers(string value, string expected)
    {
        Assert.Equal(expected, DatabasePrincipalProvisioner.QuoteIdentifier(value));
    }

    [Fact]
    public void QuoteIdentifier_rejects_empty_identifier()
    {
        Assert.Throws<ArgumentException>(() => DatabasePrincipalProvisioner.QuoteIdentifier("  "));
    }

    [Fact]
    public void QuoteLiteral_escapes_sql_string_delimiters()
    {
        Assert.Equal("N'it''s-safe'", DatabasePrincipalProvisioner.QuoteLiteral("it's-safe"));
    }

    [Fact]
    public void MigrationConfiguration_requires_administrative_connection_string()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            MigrationConfiguration.Load(_ => null));

        Assert.Equal("MIGRATION_CONNECTION_STRING must be configured.", exception.Message);
    }

    [Fact]
    public void MigrationConfiguration_requires_application_database_password()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            MigrationConfiguration.Load(variable => variable switch
            {
                "MIGRATION_CONNECTION_STRING" => "Server=db;Database=VisoERP;User Id=sa;Password=example;TrustServerCertificate=True",
                _ => null
            }));

        Assert.Equal("APP_DB_PASSWORD must be configured.", exception.Message);
    }
}
