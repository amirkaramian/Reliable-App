using System.ComponentModel.DataAnnotations;

namespace MyReliableSite.Shared.DTOs.Identity;

public class ResetPasswordRequest
{
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }

    public string Token { get; set; }
}

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
}

public class ChangePasswordOtherUserRequest
{
    public string UserId { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
}