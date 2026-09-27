using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using VisoERP.Web.Health;

namespace VisoERP.Tests.Health;

public sealed class DatabaseHealthEndpointTests
{
    [Fact]
    public async Task Health_returns_ok_when_database_is_available()
    {
        await using var app = CreateApplication(databaseAvailable: true);
        await app.StartAsync();

        var response = await app.GetTestClient().GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("healthy", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Health_returns_service_unavailable_when_database_is_unavailable()
    {
        await using var app = CreateApplication(databaseAvailable: false);
        await app.StartAsync();

        var response = await app.GetTestClient().GetAsync("/health");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal("unavailable", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public void Forwarded_proxy_configuration_trusts_only_the_declared_proxy()
    {
        var options = new ForwardedHeadersOptions();

        ForwardedProxyConfiguration.Configure(options, "172.30.0.1", required: true);

        Assert.Equal(ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto, options.ForwardedHeaders);
        Assert.Equal(1, options.ForwardLimit);
        Assert.Contains(options.KnownProxies, proxy => proxy.ToString() == "172.30.0.1");
        Assert.Empty(options.KnownIPNetworks);
    }

    [Fact]
    public void Forwarded_proxy_configuration_rejects_an_invalid_required_address()
    {
        var options = new ForwardedHeadersOptions();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            ForwardedProxyConfiguration.Configure(options, "not-an-ip", required: true));

        Assert.Equal("FORWARDED_PROXY_IP must be a valid IP address in production.", exception.Message);
    }

    private static WebApplication CreateApplication(bool databaseAvailable)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddSingleton<IDatabaseConnectionProbe>(new StubDatabaseConnectionProbe(databaseAvailable));

        var app = builder.Build();
        app.MapDatabaseHealthCheck();
        return app;
    }

    private sealed class StubDatabaseConnectionProbe(bool databaseAvailable) : IDatabaseConnectionProbe
    {
        public Task<bool> CanConnectAsync(CancellationToken cancellationToken) => Task.FromResult(databaseAvailable);
    }
}
