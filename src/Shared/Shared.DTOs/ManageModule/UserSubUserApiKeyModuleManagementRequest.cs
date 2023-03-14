namespace MyReliableSite.Shared.DTOs.ManageModule;

public class UpdateSubUserApiKeyModuleManagementRequest : IMustBeValid
{
    public string Name { get; set; }
    public string PermissionDetail { get; set; }
    public string Tenant { get; set; }
    public bool IsActive { get; set; }
    public string ApiKeyId { get; set; }
}