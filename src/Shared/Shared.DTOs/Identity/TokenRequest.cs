namespace MyReliableSite.Shared.DTOs.Identity;

public record OTPRequest(string Email, string UserName, string OTPCode);