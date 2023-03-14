using Hangfire;
using Hangfire.Console.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Infrastructure.Common.Extensions;
using MyReliableSite.Infrastructure.Common.Services;
using MyReliableSite.Infrastructure.HangFire;
using MyReliableSite.Infrastructure.Identity.Extensions;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Localizer;
using MyReliableSite.Infrastructure.Mappings;
using MyReliableSite.Infrastructure.ManageModules;
using MyReliableSite.Infrastructure.Multitenancy;
using MyReliableSite.Infrastructure.Persistence.Contexts;
using MyReliableSite.Infrastructure.Seeders;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Infrastructure.Identity;
using MaintenanceModeMiddleware.Extensions;
using MaintenanceModeMiddleware.StateStore;

namespace MyReliableSite.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config, string webPortalName)
    {
        MapsterSettings.Configure();
        if (config.GetSection("CacheSettings:PreferRedis").Get<bool>())
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config.GetSection("CacheSettings:RedisURL").Get<string>();
                options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions()
                {
                    AbortOnConnectFail = true
                };
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        services.TryAdd(ServiceDescriptor.Singleton<ICacheService, CacheService>());
        services.AddSeeders();

        services.AddMiddlewareIpRateLimit(config);
        services.AddMiddlewareCurrentUser();
        services.AddMiddlewareTenant();
        services.AddScoped<ModuleMiddleware>();
        services.AddScoped<UserModuleMiddleware>();
        services.AddScoped<ApiKeyMiddleware>();
        services.AddHealthCheckExtension();
        services.AddLocalization();
        services.AddServices();
        services.AddSettings(config);
        services.AddPermissions();
        services.AddIdentity(config);
        services.AddHangfireServer(options =>
        {
            var optionsServer = services.GetOptions<BackgroundJobServerOptions>("HangFireSettings:Server");
            options.HeartbeatInterval = optionsServer.HeartbeatInterval;
            options.Queues = optionsServer.Queues;
            options.SchedulePollingInterval = optionsServer.SchedulePollingInterval;
            options.ServerCheckInterval = optionsServer.ServerCheckInterval;
            options.ServerName = optionsServer.ServerName;
            options.ServerTimeout = optionsServer.ServerTimeout;
            options.ShutdownTimeout = optionsServer.ShutdownTimeout;
            options.WorkerCount = optionsServer.WorkerCount;
        });
        services.AddHangfireConsoleExtensions();
        services.AddHangFireService();
        services.AddMultitenancy<TenantManagementDbContext, ApplicationDbContext>(config);

        // if (webPortalName == "Admin")
        {
            services.AddModules<ApplicationDbContext>(config);
        }

        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddMiddlewares();
        services.AddSwaggerDocumentation(webPortalName);
        services.AddCorsPolicy();
        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
        });
        services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
        services.AddNotifications();
        services.AddMaintenance();

        // add webhooks
        services.AddWebhooks(opt =>
        {
            // default is "webhooks"
            opt.RoutePrefix = "wh";
        });
        return services;
    }

    public static IServiceCollection AddPermissions(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
            .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        return services;
    }
}
