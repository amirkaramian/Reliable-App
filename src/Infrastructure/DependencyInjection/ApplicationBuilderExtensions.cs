using AspNetCoreRateLimit;
using Hangfire;
using MaintenanceModeMiddleware.Configuration.Enums;
using MaintenanceModeMiddleware.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using MyReliableSite.Infrastructure.Common.Extensions;
using MyReliableSite.Infrastructure.Hubs;
using MyReliableSite.Infrastructure.Identity;
using MyReliableSite.Infrastructure.Identity.Extensions;
using MyReliableSite.Infrastructure.ManageModules;
using MyReliableSite.Infrastructure.Multitenancy;
using MyReliableSite.Infrastructure.Swagger;
using System.Globalization;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("MyReliableSite.Admin.API")]
[assembly: InternalsVisibleTo("MyReliableSite.Client.API")]

namespace MyReliableSite.Infrastructure.DependencyInjection;

internal static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app, IConfiguration config, string webPortalName)
    {
        app.Use(async (context, next) =>
        {
            // perform some verification
            context.Items["webPortalName"] = webPortalName;
            await next.Invoke();
        });
        var options = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(new CultureInfo("en-US"))
        };
        app.UseRequestLocalization(options);
        app.UseStaticFiles();
        app.UseStaticFiles(new StaticFileOptions()
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Files")),
            RequestPath = new PathString("/Files")
        });

        // app.UseBlockingDetection();
        app.UseIpRateLimiting();
        app.UseClientRateLimiting();

        app.UseMiddlewares(config);
        app.UseRouting();
        app.UseCors("CorsPolicy");
        app.UseAuthentication();
        app.UseMiddlewareCurrentUser();

        app.UseMiddlewareTenant();
        app.UseMiddleware<ApiKeyMiddleware>();
        app.UseMiddleware<ModuleMiddleware>();
        app.UseMiddleware<UserModuleMiddleware>();
        app.UseAuthorization();

        var configDashboard = config.GetSection("HangFireSettings:Dashboard").Get<DashboardOptions>();
        app.UseHangfireDashboard(config["HangFireSettings:Route"], new DashboardOptions
        {
            DashboardTitle = configDashboard.DashboardTitle,
            StatsPollingInterval = configDashboard.StatsPollingInterval,
            AppPath = configDashboard.AppPath

            // ** OPtional BasicAuthAuthorizationFilter **
            // Authorization = new[] { new BasicAuthAuthorizationFilter(
            //    new BasicAuthAuthorizationFilterOptions {
            //        RequireSsl = false,
            //        SslRedirect = false,
            //        LoginCaseSensitive = true,
            //        Users = new []
            //        {
            //            new BasicAuthAuthorizationUser
            //            {
            //                Login = config["HangFireSettings:Credentiales:User"],
            //                PasswordClear =  config["HangFireSettings:Credentiales:Password"]
            //            }
            //        }
            //    })
            // }
        });

        app.UseMaintenance(options =>
        {
            options.UseNoDefaultValues();
            options.BypassFileExtensions(new string[] { "css", "jpg", "png", "gif", "svg", "js" });
            string[] bypassPaths = new string[] { "/api/ws/signalRHub", "/swagger/index.html", "/api/maintenance", "/api/identity", "/api/token" };
            var bypassPathStrings = bypassPaths
                        .Select(s => new PathString(s));
            options.BypassUrlPaths(bypassPathStrings);
            options.BypassUserRoles(new string[] { "Admin", "Client" });
            options.UseDefaultResponse();
            options.BypassUser("Admin");
            options.BypassUser("Client");
        });
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers().RequireAuthorization();
            endpoints.MapHangfireDashboard();
            endpoints.MapHealthChecks("/api/health").RequireAuthorization();
            endpoints.MapHub<NotificationHub>(pattern: "/api/ws/signalRHub", options =>
            {
                options.Transports = HttpTransportType.WebSockets |
                HttpTransportType.LongPolling | HttpTransportType.ServerSentEvents;
                options.CloseOnAuthenticationExpiration = true;
            });
        });
        app.UseSwaggerDocumentation(config);
        return app;
    }
}
