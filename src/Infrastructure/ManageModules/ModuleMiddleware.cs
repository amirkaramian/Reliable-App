using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MyReliableSite.Application.ManageModule.Interfaces;
using MyReliableSite.Application.Settings;
using Newtonsoft.Json;

namespace MyReliableSite.Infrastructure.ManageModules;

public class ModuleMiddleware : IMiddleware
{
    private readonly IModuleManagementService _service;
    private readonly IConfiguration _config;
    private readonly MiddlewareSettings _middlewareSettings;
    public ModuleMiddleware(IModuleManagementService service, IConfiguration config, IOptions<MiddlewareSettings> middlewareSettings)
    {
        _service = service;
        _config = config;
        _middlewareSettings = middlewareSettings.Value;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!ExcludePath(context))
        {
            try
            {
                string module = ModuleResolver.Resolver(context);
                string moduleAction = ModuleActionResolver.Resolver(context);
                bool isAllowed = true;
                if (!string.IsNullOrEmpty(module) && !ExcludeModules(module))
                {
                    if (!string.IsNullOrEmpty(moduleAction))
                    {

                        string action = moduleAction;
                        string method = context.Request.Method;
                        if (context.Request != null && context.Request.Headers != null && context.Request.Headers.TryGetValue("tenant", out var tenant))
                        {
                            var modules = await _service.GetModuleManagementByTenantIdAsync(tenant.First());
                            if (modules != null && modules.Data != null & modules.Data.Count > 0)
                            {
                                var permissionForThisController = modules.Data.FirstOrDefault(m => m.Name == module);
                                if (permissionForThisController != null && permissionForThisController.IsActive)
                                {
                                    Dictionary<string, bool> permissions = JsonConvert.DeserializeObject<Dictionary<string, bool>>(permissionForThisController.PermissionDetail);

                                    bool isActive;
                                    switch (method)
                                    {
                                        case "GET":
                                            if (!permissions.TryGetValue(action.ToLower() == "search" ? "Search" : "View", out isActive))
                                            {
                                                isActive = false;
                                            }

                                            if (!isActive)
                                            {
                                                isAllowed = false;
                                            }

                                            break;
                                        case "POST":
                                            if (!permissions.TryGetValue(action.ToLower() == "search" ? "Search" : action.ToLower() == "register" ? "Register" : "Create", out isActive))
                                            {
                                                isActive = false;
                                            }

                                            if (!isActive)
                                            {
                                                isAllowed = false;
                                            }

                                            break;
                                        case "DELETE":
                                            if (!permissions.TryGetValue(action.ToLower() == "delete" ? "Delete" : "Remove", out isActive))
                                            {
                                                isActive = false;
                                            }

                                            if (!isActive)
                                            {
                                                isAllowed = false;
                                            }

                                            break;
                                        case "PUT":
                                            if (!permissions.TryGetValue("Update", out isActive))
                                            {
                                                isActive = false;
                                            }

                                            if (!isActive)
                                            {
                                                isAllowed = false;
                                            }

                                            break;
                                        case "PATCH":
                                            if (!permissions.TryGetValue("Patch", out isActive))
                                            {
                                                isActive = false;
                                            }

                                            if (!isActive)
                                            {
                                                isAllowed = false;
                                            }

                                            break;
                                        default:
                                            isAllowed = false;
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                context.Response.StatusCode = 404;
                                await context.Response.WriteAsync("This module is not exists in Modules. (Using ModuleMiddleware)");
                                return;
                            }
                        }
                        else
                        {
                            context.Response.StatusCode = 404;
                            await context.Response.WriteAsync("tenant is missing. (Using ModuleMiddleware)");
                            return;
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync("Unauthorized to consume with null controller & action. (Using ApiKeyMiddleware)");
                        isAllowed = false;
                    }

                    if (!isAllowed)
                    {
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync("Unauthorized to consume this module. (Using ModuleMiddleware)");
                        return;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        await next(context);
    }

    private bool ExcludeModules(string module)
    {
        foreach (string item in _middlewareSettings.ModuleExcludedForMiddlewareValidation.Split(",").ToList())
        {
            if (module.Equals(item))
            {
                return true;
            }
        }

        return false;
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
