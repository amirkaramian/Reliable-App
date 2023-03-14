namespace MyReliableSite.Shared.DTOs.Identity;

public class CreateAdminGroupRequest : IMustBeValid
{
    public string GroupName { get; set; }
    public bool Status { get; set; }
    public bool IsSuperAdmin { get; set; }
    public bool IsDefault { get; set; }
    public string Tenant { get; set; }
}