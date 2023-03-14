using Microsoft.Extensions.DependencyInjection;
using MyReliableSite.Application.Settings;
using MyReliableSite.Infrastructure.Multitenancy;
using Serilog;

namespace MyReliableSite.Infrastructure.Common.Extensions;

public static class SignalrExtensions
{
    internal static IServiceCollection AddNotifications(this IServiceCollection services)
    {
        ILogger logger = Log.ForContext(typeof(SignalrExtensions));

        var signalSettings = services.GetOptions<SignalSettings>("SignalRSettings");

        if (!signalSettings.UseBackplane)
        {
            services.AddSignalR();
        }
        else
        {
            var backplaneSettings = services.GetOptions<SignalSettings.Backplane>("SignalRSettings:Backplane");
            switch (backplaneSettings.Provider)
            {
                case "redis":
                    services.AddSignalR().AddStackExchangeRedis(backplaneSettings.StringConnection, options =>
                    {
                        options.Configuration.AbortOnConnectFail = false;
                    });
                    break;

                default:
                    throw new Exception($"SignalR backplane Provider {backplaneSettings.Provider} is not supported.");
            }

            logger.Information($"SignalR Backplane Current Provider: {backplaneSettings.Provider}.");
        }

        return services;
    }
}