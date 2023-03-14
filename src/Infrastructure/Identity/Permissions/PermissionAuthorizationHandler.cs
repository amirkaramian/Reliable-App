using Microsoft.AspNetCore.Authorization;
using MyReliableSite.Application.Abstractions.Services.Identity;
using System.Security.Claims;

namespace MyReliableSite.Infrastructure.Identity.Permissions;

internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IRoleClaimsService _permissionService;

    public PermissionAuthorizationHandler(IRoleClaimsService permissionService)
    {
        _permissionService = permissionService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User == null)
        {
            await Task.CompletedTask;
        }

        string userId = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId == null && (context.User.Identity.AuthenticationType == "admin-api-key" || context.User.Identity.AuthenticationType == "user-api-key" || context.User.Identity.AuthenticationType == "gen-api-key")
            && await _permissionService.RoleHasPermissionAsync(context.User.Claims.Select(m => m.Value).ToList<string>(), context.User.Identity.Name, requirement.Permission))
        {
            context.Succeed(requirement);
            await Task.CompletedTask;
        }
        else if (userId == null && (context.User.Identity.AuthenticationType == null)
           && await _permissionService.RoleHasPermissionAsync(context.User.Claims.Select(m => m.Value).ToList<string>(), context.User.Identity.Name, requirement.Permission))
        {
            context.Succeed(requirement);
            await Task.CompletedTask;
        }
        else if (await _permissionService.HasPermissionAsync(userId, requirement.Permission))
        {
            context.Succeed(requirement);
            await Task.CompletedTask;
        }
    }
}
