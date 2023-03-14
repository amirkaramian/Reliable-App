namespace MyReliableSite.Shared.DTOs.ManageModule;

public class CreateUserModuleManagementRequest : IMustBeValid
{
    public string Name { get; set; }
    public string PermissionDetail { get; set; }
    public string Tenant { get; set; }
    public bool IsActive { get; set; }
    public string UserId { get; set; }
}
