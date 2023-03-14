namespace MyReliableSite.Shared.DTOs.Identity;

public record TokenRequest(string Email, string UserName, string Password, bool TrustDevice);