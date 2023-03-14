using MyReliableSite.Shared.DTOs.Departments;
using MyReliableSite.Shared.DTOs.ManageModule;

namespace MyReliableSite.Shared.DTOs.Identity;

public class UserDetailsEXL
{
    public Guid Id { get; set; }

    public string UserName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }

    public bool IsActive { get; set; } = true;

    public bool EmailConfirmed { get; set; }

    public string PhoneNumber { get; set; }

    public string ImageUrl { get; set; }
    public bool Status { get; set; }
    public UserRolesResponse UserRolesResponse { get; set; }
    public string AdminGroupId { get; set; }
    public int requestsPerIPOverwrite { get; set; }
    public int requestsIntervalPerIPAfterLimitInSecondsOverwrite { get; set; }
    public string Base64Image { get; set; }
    public string CompanyName { get; set; }
    public string Tenant { get; set; }
    public string BrandId { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string City { get; set; }
    public string State_Region { get; set; }
    public string ZipCode { get; set; }
    public string Country { get; set; }
    public string ParentID { get; set; }
    public bool IsDeleted { get; set; }
    public string LastLoggedIn { get; set; }
    public string CreatedOn { get; set; }
    public List<string> IpAddresses { get; set; }
    public ICollection<DepartmentDto> Departments { get; set; }
    public ICollection<UserModuleDto> UserModules { get; set; }
    public List<Guid> DepartmentIds { get; set; }
    public int RecordsToDisplay { get; set; }
}