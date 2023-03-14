using Microsoft.AspNetCore.Authorization;

namespace MyReliableSite.Infrastructure.Identity.Permissions;

public class MustHavePermission : AuthorizeAttribute
{
    public MustHavePermission(string permission)
    {
        Policy = permission;
    }
}
