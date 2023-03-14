using System.ComponentModel.DataAnnotations;

namespace MyReliableSite.Shared.DTOs.Identity;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}