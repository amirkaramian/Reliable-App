using Microsoft.Extensions.DependencyInjection;
using MyReliableSite.Application.Settings;
using MyReliableSite.Infrastructure.Multitenancy;

namespace MyReliableSite.Infrastructure.Common.Extensions;

public static class CorsExtensions
{
    internal static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        var corsSettings = services.GetOptions<CorsSettings>(nameof(CorsSettings));
        return services.AddCors(opt =>
        {
            opt.AddPolicy("CorsPolicy", policy =>
            {
                if (corsSettings.EnableAnyOriginCors)
                {
                    policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();

                }
                else
                {
                    policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(new string[] { corsSettings.Angular, corsSettings.Blazor });
                }
            });
        });
    }
}