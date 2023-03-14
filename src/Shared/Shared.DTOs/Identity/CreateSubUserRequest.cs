using MyReliableSite.Shared.DTOs.ManageModule;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Identity;
public class CreateSubUserRequest
{
    [Required]
    public string FullName { get; set; }

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

    public List<CreateSubUserModuleManagementRequest> SubUserModules { get; set; }
}
