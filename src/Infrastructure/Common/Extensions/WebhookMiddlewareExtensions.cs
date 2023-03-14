using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MyReliableSite.Infrastructure.Common.Extensions;

public static class WebhookMiddlewareExtensions
{
    public static IServiceCollection AddWebhooks(this IServiceCollection services, Action<WebhookOptions> spaceAction = null)
    {
        var options = new WebhookOptions();

        services.Configure<RouteOptions>(opt =>
        {
            opt.ConstraintMap.Add("webhookRoutePrefix", typeof(WebhookRoutePrefixConstraint));
        });
        spaceAction?.Invoke(options);
        services.AddSingleton(options);

        return services;
    }
}

public class WebhookRoutePrefixConstraint : IRouteConstraint
{
    public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
    {
        if (httpContext != null && values.TryGetValue("prefix", out object value) && value is string actual)
        {
            var options = (WebhookOptions)httpContext
                .RequestServices
                .GetService(typeof(WebhookOptions));

            // urls are case sensitive
            string expected = options?.RoutePrefix;
            return expected == actual;
        }

        return false;
    }
}

public class WebhookOptions
{
    public string RoutePrefix { get; set; } = "webhooks";
}
