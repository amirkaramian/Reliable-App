using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.ManageModule;

public class AdminGroupModule : AuditableEntity, IMustHaveTenant
{
    public string Name { get; set; }
    public string PermissionDetail { get; set; }
    public string Tenant { get; set; }
    public bool IsActive { get; set; }
    public string AdminGroupId { get; set; }
    public AdminGroupModule(string name, string permissionDetail, string tenant, bool isActive, string adminGroupId)
    {
        PermissionDetail = permissionDetail;
        Name = name;
        Tenant = tenant;
        IsActive = isActive;
        AdminGroupId = adminGroupId;
    }

    public AdminGroupModule Update(string name, string permissionDetail, bool isActive, string adminGroupId)
    {
        if (permissionDetail != null && !PermissionDetail.NullToString().Equals(permissionDetail)) PermissionDetail = permissionDetail;
        if (name != null && !Name.NullToString().Equals(name)) Name = name;
        if (adminGroupId != null && !AdminGroupId.NullToString().Equals(adminGroupId)) AdminGroupId = adminGroupId;
        if (IsActive != isActive) IsActive = isActive;
        return this;
    }
}
