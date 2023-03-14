using Microsoft.AspNetCore.Identity;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Infrastructure.Identity.Models;

public class ApplicationRole : IdentityRole
{
    public string Description { get; set; }
    public string Tenant { get; set; }

    public ApplicationRole()
        : base()
    {
    }

    public ApplicationRole(string roleName, string tenant, string description = null)
        : base(roleName)
    {
        Description = description;
        NormalizedName = roleName.ToUpper();
        Tenant = tenant;
    }
}