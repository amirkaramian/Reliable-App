using Microsoft.AspNetCore.Identity;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using MyReliableSite.Domain.Departments;
using MyReliableSite.Domain.Identity;

namespace MyReliableSite.Infrastructure.Identity.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public string Tenant { get; set; }
    public bool MFARequest { get; set; }
    public string OTPCode { get; set; }
    public string FullName { get; set; }
    public bool Status { get; set; } = true;
    public string BrandId { get; set; }
    public string CompanyName { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string City { get; set; }
    public string State_Region { get; set; }
    public string ZipCode { get; set; }
    public string Country { get; set; }
    public string ParentID { get; set; }
    public int RequestsPerIPOverwrite { get; set; } = 50;
    public int RequestsIntervalPerIPAfterLimitInSecondsOverwrite { get; set; } = 60;
    public bool IsDeleted { get; set; }
    public string AdminGroupId { get; set; }
    public DateTime LastLoggedIn { get; set; }
    public int OldUserId { get; set; }
    public ICollection<Department> Departments { get; set; }
    public DateTime CreatedOn { get; set; }
    public int RecordsToDisplay { get; set; }
    public bool CanTakeOrders { get; set; }
    public bool AvailableForOrders { get; set; }
    public bool CanTakeTickets { get; set; }

}

