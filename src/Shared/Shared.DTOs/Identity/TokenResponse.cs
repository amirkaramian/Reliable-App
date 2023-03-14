namespace MyReliableSite.Shared.DTOs.Identity;

public record TokenResponse(string Token, string RefreshToken, DateTime RefreshTokenExpiryTime, bool isAdminAsClient = false, string currentAdminUserID = "");