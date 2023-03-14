using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using MyReliableSite.Domain.ManageUserApiKey;

namespace MyReliableSite.Domain.ManageModule;

public class UserApiKeyModule : AuditableEntity, IMustHaveTenant
{
    public string Name { get; set; }
    public string PermissionDetail { get; set; }
    public string Tenant { get; set; }
    public bool IsActive { get; set; }
    public Guid APIKeyPairId { get; set; }
    public APIKeyPair APIKeyPair { get; set; }
    public UserApiKeyModule(string name, string permissionDetail, string tenant, bool isActive, Guid aPIKeyPairId)
    {
        PermissionDetail = permissionDetail;
        Name = name;
        Tenant = tenant;
        IsActive = isActive;
        APIKeyPairId = aPIKeyPairId;
    }

    public UserApiKeyModule Update(string name, string permissionDetail, bool isActive, Guid aPIKeyPairId)
    {
        if (permissionDetail != null && !PermissionDetail.NullToString().Equals(permissionDetail)) PermissionDetail = permissionDetail;
        if (name != null && !Name.NullToString().Equals(name)) Name = name;
        if (APIKeyPairId != aPIKeyPairId) APIKeyPairId = aPIKeyPairId;
        if (IsActive != isActive) IsActive = isActive;
        return this;
    }
}