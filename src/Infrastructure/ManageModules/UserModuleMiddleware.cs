using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.ManageModule.Interfaces;
using MyReliableSite.Application.Settings;
using MyReliableSite.Infrastructure.Identity.Extensions;
using MyReliableSite.Infrastructure.Identity.Models;
using Newtonsoft.Json;

namespace MyReliableSite.Infrastructure.ManageModules;

public class UserModuleMiddleware : IMiddleware
{
    private readonly IUserModuleManagementService _service;
    private readonly IAdminGroupModuleManagementService _adminservice;
    private readonly IConfiguration _config;
    private readonly ICurrentUser _currentUser;
    private readonly MiddlewareSettings _middlewareSettings;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserModuleMiddleware(IUserModuleManagementService service, IAdminGroupModuleManagementService adminservice, UserManager<ApplicationUser> userManager, IConfiguration config, ICurrentUser currentUser, IOptions<MiddlewareSettings> middlewareSettings)
    {
        _service = service;
        _adminservice = adminservice;
        _config = config;
        _currentUser = currentUser;
        _middlewareSettings = middlewareSettings.Value;
        _userManager = userManager;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!ExcludePath(context))
        {
            try
            {
                Guid currentUserId = _currentUser.GetUserId();
                string tenantId = ResolveFromHeader(context);
                if (currentUserId != Guid.Empty)
                {
                    string module = ModuleResolver.Resolver(context);
                    string moduleAction = ModuleActionResolver.Resolver(context);
                    if (!string.IsNullOrEmpty(module) && !ExcludeModules(module))
                    {

                        string action = moduleAction;
                        bool isAllowed = true;

                        if (!string.IsNullOrEmpty(action))
                        {
                            string method = context.Request.Method;

                            var modules = await _service.GetUserModuleManagementByUserIdAsync(currentUserId.ToString());

                            if (modules != null && modules.Data != null & modules.Data.Count > 0)
                            {

                                var permissionForThisController = modules.Data.FirstOrDefault(m => m.Name.Equals(module, StringComparison.OrdinalIgnoreCase));
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
                                            if (!permissions.TryGetValue("Remove", out isActive))
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
                                else
                                {
                                    isAllowed = permissionForThisController != null && permissionForThisController.IsActive;
                                }

                                if (isAllowed)
                                {
                                    var user = _userManager.Users.First(u => u.Id == currentUserId.ToString());
                                    if (!string.IsNullOrEmpty(user.AdminGroupId))
                                    {
                                        var adminGroup = await _adminservice.GetAdminGroupModuleManagementByAdminGroupIdAsync(user.AdminGroupId);
                                        if (adminGroup != null && adminGroup.Data != null & adminGroup.Data.Count > 0)
                                        {
                                            var permissionadminGroup = adminGroup.Data.FirstOrDefault(m => m.Name == module);
                                            if (isAllowed && permissionadminGroup != null && permissionadminGroup.IsActive)
                                            {
                                                Dictionary<string, bool> permissions = JsonConvert.DeserializeObject<Dictionary<string, bool>>(permissionadminGroup.PermissionDetail);

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
                                                        if (!permissions.TryGetValue("Remove", out isActive))
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
                                            else
                                            {
                                                isAllowed = permissionadminGroup != null && permissionadminGroup.IsActive;
                                            }
                                        }
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
                            await context.Response.WriteAsync("tenant is missing. (Using UserModuleMiddleware)");
                            return;
                        }

                        if (!isAllowed)
                        {
                            context.Response.StatusCode = 404;
                            await context.Response.WriteAsync("Unauthorized to consume this module. (Using UserModuleMiddleware)");
                            return;
                        }
                    }
                    else if (tenantId.ToLower() == "client")
                    {
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync("Unauthorized to consume this module. (Using UserModuleMiddleware)");
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

    private static string ResolveFromHeader(HttpContext context)
    {
        context.Request.Headers.TryGetValue("tenant", out var tenantFromHeader);
        return tenantFromHeader;
    }
}