using AspNetCoreRateLimit;
using AspNetCoreRateLimit.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyReliableSite.Infrastructure.Identity.Extensions;

public static class IpRateLimitExtension
{
    internal static IApplicationBuilder UseMiddlewareIpRateLimit(this IApplicationBuilder app)
    {
        app.UseIpRateLimiting();

        return app;
    }

    internal static IServiceCollection AddMiddlewareIpRateLimit(this IServiceCollection services, IConfiguration config)
    {

        // configure ip rate limiting middleware
        services.Configure<IpRateLimitOptions>(config.GetSection("IpRateLimiting"));
        services.Configure<IpRateLimitPolicies>(config.GetSection("IpRateLimitPolicies"));

        // configure client rate limiting middleware
        services.Configure<ClientRateLimitOptions>(config.GetSection("ClientRateLimiting"));
        services.Configure<ClientRateLimitPolicies>(config.GetSection("ClientRateLimitPolicies"));

        // inject counter and rules stores
        // register stores
        // services.AddInMemoryRateLimiting();
        services.AddDistributedRateLimiting<AsyncKeyLockProcessingStrategy>();

        using var serviceProvider = services.BuildServiceProvider();

        // get the ClientPolicyStore instance
        var clientPolicyStore = serviceProvider.GetRequiredService<IClientPolicyStore>();

        // seed Client data from appsettings
        _ = clientPolicyStore.SeedAsync().ConfigureAwait(true);

        // get the IpPolicyStore instance
        var ipPolicyStore = serviceProvider.GetRequiredService<IIpPolicyStore>();

        // seed IP data from appsettings
        _ = ipPolicyStore.SeedAsync().ConfigureAwait(true);

        // services.AddDistributedRateLimiting<AsyncKeyLockProcessingStrategy>();

        // services.AddDistributedRateLimiting<RedisProcessingStrategy>();
        // services.AddRedisRateLimiting();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        return services;
    }
}
