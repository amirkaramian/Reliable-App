using Microsoft.AspNetCore.Http;
using MyReliableSite.Infrastructure.Identity.Extensions;

namespace MyReliableSite.Infrastructure.ManageModules;

public static class ModuleResolver
{
    public static string Resolver(HttpContext context)
    {

        string moduleName = ResolveFromHeader(context);
        if (!string.IsNullOrEmpty(moduleName))
        {
            return moduleName;
        }

        moduleName = ResolveFromQuery(context);
        if (!string.IsNullOrEmpty(moduleName))
        {
            return moduleName;
        }

        return default;
    }

    private static string ResolveFromHeader(HttpContext context)
    {
        context.Request.Headers.TryGetValue("modulename", out var moduleFromHeader);
        return moduleFromHeader;
    }

    private static string ResolveFromQuery(HttpContext context)
    {
        context.Request.Query.TryGetValue("modulename", out var moduleFromQueryString);
        return moduleFromQueryString;
    }
}

public static class ModuleActionResolver
{
    public static string Resolver(HttpContext context)
    {

        string moduleName = ResolveFromHeader(context);
        if (!string.IsNullOrEmpty(moduleName))
        {
            return moduleName;
        }

        moduleName = ResolveFromQuery(context);
        if (!string.IsNullOrEmpty(moduleName))
        {
            return moduleName;
        }

        return default;
    }

    private static string ResolveFromHeader(HttpContext context)
    {
        context.Request.Headers.TryGetValue("moduleactionname", out var moduleFromHeader);
        return moduleFromHeader;
    }

    private static string ResolveFromQuery(HttpContext context)
    {
        context.Request.Query.TryGetValue("moduleactionname", out var moduleFromQueryString);
        return moduleFromQueryString;
    }
}