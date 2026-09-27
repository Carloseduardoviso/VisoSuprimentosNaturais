using System.Net;
using Microsoft.AspNetCore.HttpOverrides;

namespace VisoERP.Web.Health;

public static class ForwardedProxyConfiguration
{
    public static void Configure(ForwardedHeadersOptions options, string? proxyIp, bool required)
    {
        options.KnownProxies.Clear();
        options.KnownIPNetworks.Clear();

        if (string.IsNullOrWhiteSpace(proxyIp) && !required) return;

        if (!IPAddress.TryParse(proxyIp, out var parsedProxyIp))
            throw new InvalidOperationException("FORWARDED_PROXY_IP must be a valid IP address in production.");

        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        options.ForwardLimit = 1;
        options.KnownProxies.Add(parsedProxyIp);
    }
}
