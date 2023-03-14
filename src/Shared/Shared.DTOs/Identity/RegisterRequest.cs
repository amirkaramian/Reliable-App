using System.ComponentModel.DataAnnotations;

namespace MyReliableSite.Shared.DTOs.Identity;

public class RegisterRequest
{
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string UserName { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }

    public string PhoneNumber { get; set; }
}

public class RegisterAdminRequest
{
    [Required]
    [MinLength(6)]
    public string UserName { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string FullName { get; set; }

    public bool Status { get; set; }

    // [Required]
    public List<string> IpAddresses { get; set; }
    public string AdminGroupId { get; set; }
    public List<Guid> DepartmentIds { get; set; }

}

public class ReCaptchaClientRequest
{
    [Required]
    public string ReCaptchaToken { get; set; }
}

public class RegisterClientRequest
{
    [Required]
    public string FullName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }

    [Required]
    public string CompanyName { get; set; }

    [Required]
    public string Address1 { get; set; }

    public string Address2 { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    public string State_Region { get; set; }

    [Required]
    public string ZipCode { get; set; }
    [Required]
    public string Country { get; set; }
    public string PhoneNumber { get; set; }
    public string BrandId { get; set; }
    public bool Status { get; set; }

    public string ParentID { get; set; }

    public int OldUserId { get; set; }
}