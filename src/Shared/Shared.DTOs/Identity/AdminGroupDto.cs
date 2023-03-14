namespace MyReliableSite.Shared.DTOs.Identity;

public class AdminGroupDto : IDto
{
    public Guid Id { get; set; }
    public string GroupName { get; set; }
    public bool Status { get; set; }
    public bool IsDefault { get; set; }
    public bool IsSuperAdmin { get; set; }
    public DateTime CreatedOn { get; set; }
    public int UserCount { get; set; }
}