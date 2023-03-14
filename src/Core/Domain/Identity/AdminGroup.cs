using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.Identity;

public class AdminGroup : AuditableEntity, IMustHaveTenant
{
    public string GroupName { get; set; }
    public bool Status { get; set; }
    public bool IsDefault { get; set; }
    public bool IsSuperAdmin { get; set; } = false;
    public string Tenant { get; set; }

    public AdminGroup(string groupName, bool status, bool isDefault, bool isSuperAdmin, string tenant)
    {
        GroupName = groupName;
        Status = status;
        Tenant = tenant;
        IsDefault = isDefault;
        IsSuperAdmin = isSuperAdmin;
    }

    public AdminGroup Update(string groupName, bool status, bool isDefault, bool isSuperAdmin, string tenant)
    {
        if (tenant != null && !Tenant.NullToString().Equals(tenant)) Tenant = tenant;
        if (groupName != null && !GroupName.NullToString().Equals(groupName)) GroupName = groupName;
        if (Status != status) Status = status;
        if (IsDefault != isDefault) IsDefault = isDefault;
        if (IsSuperAdmin != isSuperAdmin) IsSuperAdmin = isSuperAdmin;

        return this;
    }

}
