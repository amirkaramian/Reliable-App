using MyReliableSite.Shared.DTOs.ManageModule;

namespace MyReliableSite.Shared.DTOs.ManageUserApiKey;

public class APIKeyPairDto : IDto
{
    public Guid Id { get; set; }
    public string ApplicationKey { get; set; }
    public string UserIds { get; set; }
    public string SafeListIpAddresses { get; set; }
    public DateTime ValidTill { get; set; }
    public bool StatusApi { get; set; }
    public string Tenant { get; set; }
    public string Label { get; set; }

    public ICollection<UserApiKeyModuleDto> UserApiKeyModules { get; set; }
}
