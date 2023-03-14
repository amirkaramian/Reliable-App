using MyReliableSite.Shared.DTOs.ManageModule;

namespace MyReliableSite.Shared.DTOs.ManageUserApiKey;

public class UpdateAPIKeyPairRequest : IMustBeValid
{
    public string ApplicationKey { get; set; }
    public string UserIds { get; set; }
    public string SafeListIpAddresses { get; set; }
    public List<UpdateUserApiKeyModuleManagementRequest> UserApiKeyModules { get; set; }
    public DateTime ValidTill { get; set; }
    public bool StatusApi { get; set; }
    public string Tenant { get; set; }
    public string Label { get; set; }
}

public class UpdateAPIKeyPairPermissionRequest : IMustBeValid
{
    public List<UpdateUserApiKeyModuleManagementRequest> UserApiKeyModules { get; set; }
}