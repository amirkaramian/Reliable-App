using Microsoft.Extensions.DependencyInjection;
using MyReliableSite.Infrastructure.Multitenancy;

namespace MyReliableSite.Infrastructure.Common.Extensions;

public static class HealthCheckExtensions
{
    internal static IServiceCollection AddHealthCheckExtension(this IServiceCollection services)
    {
        services.AddHealthChecks().AddCheck<TenantHealthCheck>("Tenant");
        return services;
    }
}