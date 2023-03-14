using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Identity.Exceptions;
using MyReliableSite.Application.Multitenancy;
using Serilog;
using System.Net;

namespace MyReliableSite.Infrastructure.Multitenancy;

public class TenantMiddleware : IMiddleware
{
    private readonly IStringLocalizer<TenantMiddleware> _localizer;
    private readonly ITenantService _tenantService;

    public TenantMiddleware(IStringLocalizer<TenantMiddleware> localizer, ITenantService tenantService)
    {
        _localizer = localizer;
        _tenantService = tenantService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!ExcludePath(context))
        {
            Log.Logger.Information(context.Request.Path.Value.ToString());
            string tenantId = TenantResolver.Resolver(context);
            if (!string.IsNullOrEmpty(tenantId))
            {
                _tenantService.SetCurrentTenant(tenantId);
            }
            else
            {
                throw new IdentityException(_localizer["auth.failed"], statusCode: HttpStatusCode.Unauthorized);
            }
        }

        await next(context);
    }

    private bool ExcludePath(HttpContext context)
    {
        var listExclude = new List<string>()
        {
            "/files/",
            "/swagger",
            "/jobs",
            "/favicon.ico"
        };

        foreach (string item in listExclude)
        {
            if (context.Request.Path.HasValue && context.Request.Path.Value.Contains(item))
            {
                return true;
            }
        }

        return false;
    }
}
