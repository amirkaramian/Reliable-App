using MyReliableSite.Shared.DTOs.ManageModule;

namespace MyReliableSite.Shared.DTOs.ManageSubUserApiKey;

public class UpdateSubUserAPIKeyPairRequest : IMustBeValid
{
    public string ApplicationKey { get; set; }
    public string SubUserIds { get; set; }
    public string SafeListIpAddresses { get; set; }
    public List<UpdateSubUserApiKeyModuleManagementRequest> SubUserApiKeyModules { get; set; }
    public DateTime ValidTill { get; set; }
    public bool StatusApi { get; set; }
    public string Tenant { get; set; }
    public string Label { get; set; }
}

public class UpdateSubUserAPIKeyPairPermissionRequest : IMustBeValid
{
    public List<UpdateSubUserApiKeyModuleManagementRequest> UserApiKeyModules { get; set; }
}