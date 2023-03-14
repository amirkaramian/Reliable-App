using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using MyReliableSite.Domain.ManageModule;

namespace MyReliableSite.Domain.ManageUserApiKey;

public class APIKeyPair : AuditableEntity, IMustHaveTenant
{
    public string ApplicationKey { get; set; }
    public string UserIds { get; set; }
    public string SafeListIpAddresses { get; set; }
    public DateTime ValidTill { get; set; }
    public bool StatusApi { get; set; }
    public string Tenant { get; set; }
    public ICollection<UserApiKeyModule> UserApiKeyModules { get; set; }
    public string Label { get; set; }

    // public string Roles { get; set; }

    public APIKeyPair()
    {
    }

    public APIKeyPair(string applicationKey, string userIds, string safeListIpAddresses, DateTime validTill, bool statusApi, string label)
    {
        ApplicationKey = applicationKey;
        UserIds = userIds;
        SafeListIpAddresses = safeListIpAddresses;
        ValidTill = validTill;
        StatusApi = statusApi;
        Label = label;
    }

    public APIKeyPair Update(string applicationKey, string userIds, string safeListIpAddresses, DateTime validTill, bool statusApi, string label)
    {
        if (applicationKey != null && !ApplicationKey.NullToString().Equals(applicationKey)) ApplicationKey = applicationKey;
        if (label != null && !Label.NullToString().Equals(label)) Label = label;
        if (userIds != null && !UserIds.NullToString().Equals(userIds)) UserIds = userIds;
        if (safeListIpAddresses != null && !SafeListIpAddresses.NullToString().Equals(safeListIpAddresses)) SafeListIpAddresses = safeListIpAddresses;
        if (ValidTill != validTill) ValidTill = validTill;
        if (StatusApi != statusApi) StatusApi = statusApi;
        return this;
    }
}
