using MyReliableSite.Shared.DTOs.ManageModule;

namespace MyReliableSite.Shared.DTOs.ManageSubUserApiKey;

public class CreateSubUserAPIKeyPairRequest : IMustBeValid
{
    public string ApplicationKey { get; set; }
    public string SubUserIds { get; set; }
    public string SafeListIpAddresses { get; set; }

    public List<CreateSubUserApiKeyModuleManagementRequest> SubUserApiKeyModules { get; set; }
    public DateTime ValidTill { get; set; }
    public bool StatusApi { get; set; }
    public string Tenant { get; set; }
    public string Label { get; set; }
}
