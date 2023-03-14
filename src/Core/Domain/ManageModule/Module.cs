using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.ManageModule;

public class Module : AuditableEntity, IMustHaveTenant
{
    public string Name { get; set; }
    public string PermissionDetail { get; set; }
    public string Tenant { get; set; }
    public bool IsActive { get; set; }
    public Module(string name, string permissionDetail, string tenant, bool isActive)
    {
        PermissionDetail = permissionDetail;
        Name = name;
        Tenant = tenant;
        IsActive = isActive;
    }

    public Module Update(string name, string permissionDetail, bool isActive)
    {
        if (permissionDetail != null && !PermissionDetail.NullToString().Equals(permissionDetail)) PermissionDetail = permissionDetail;
        if (name != null && !Name.NullToString().Equals(name)) Name = name;
        if (IsActive != isActive) IsActive = isActive;
        return this;
    }
}