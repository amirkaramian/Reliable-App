namespace MyReliableSite.Shared.DTOs.ManageModule;

public class CreateUserApiKeyModuleManagementRequest : IMustBeValid
{
    public string Name { get; set; }
    public string PermissionDetail { get; set; }
    public string Tenant { get; set; }
    public bool IsActive { get; set; }
    public string UserApiKeyId { get; set; }
}
