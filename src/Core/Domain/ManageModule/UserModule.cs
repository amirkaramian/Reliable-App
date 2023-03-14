using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.ManageModule;

public class UserModule : AuditableEntity, IMustHaveTenant
{
    public string Name { get; set; }
    public string PermissionDetail { get; set; }
    public string Tenant { get; set; }
    public bool IsActive { get; set; }
    public string UserId { get; set; }
    public UserModule(string name, string permissionDetail, string tenant, bool isActive, string userId)
    {
        PermissionDetail = permissionDetail;
        Name = name;
        Tenant = tenant;
        IsActive = isActive;
        UserId = userId;
    }

    public UserModule Update(string name, string permissionDetail, bool isActive, string userId)
    {
        if (permissionDetail != null && !PermissionDetail.NullToString().Equals(permissionDetail)) PermissionDetail = permissionDetail;
        if (name != null && !Name.NullToString().Equals(name)) Name = name;
        if (userId != null && !UserId.NullToString().Equals(userId)) UserId = userId;
        if (IsActive != isActive) IsActive = isActive;
        return this;
    }
}