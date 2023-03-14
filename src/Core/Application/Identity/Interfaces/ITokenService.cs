using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Application.Abstractions.Services.Identity;

public interface ITokenService : ITransientService
{
    Task<IResult<TokenResponse>> GetTokenAsync(TokenRequest request, string ipAddress, string deviceName, string location, bool flgAdmin, string origin);
    Task<IResult<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress, bool flgAdmin);
    Task<IResult<TokenResponse>> LoginAdminAsClientAsync(string clientId, string ipAddress);

    Task<IResult<TokenResponse>> LoginClientAsAdminAsync(string adminId, string ipAddress);

    Task<IResult<TokenResponse>> GetTokenByOTPAsync(OTPRequest request, string ipAddress, string deviceName, string location, bool flgAdmin);
    Task<IResult> RevokeToken(string userId);
    Task<TokenResponse> GetUserUpdatedToken(string ipAddress, bool flgAdmin, string userId);
}
