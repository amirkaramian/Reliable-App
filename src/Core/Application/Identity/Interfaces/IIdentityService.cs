using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Application.Abstractions.Services.Identity;

public interface IIdentityService : ITransientService
{
    Task<IResult> verifyRecaptchaAsync(ReCaptchaClientRequest reCaptchaClientRequest);
    Task<IResult> RegisterAsync(RegisterRequest request, string origin);

    Task<IResult> RegisterAdminAsync(RegisterAdminRequest request, string origin);

    Task<IResult> RegisterClientUserAsync(RegisterClientRequest request, string origin);
    Task<IResult> RegisterImportClientUserAsync(RegisterClientRequest request, string origin);

    Task<IResult<string>> ConfirmEmailAsync(string userId, string code, string tenant);

    Task<IResult<string>> ConfirmPhoneNumberAsync(string userId, string code);

    Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin);

    Task<IResult> ResetPasswordAsync(ResetPasswordRequest request);

    Task<IResult> UpdateProfileAsync(UpdateProfileRequest request);
    Task<IResult> UpdateProfileAsync(UpdateProfileRequest request, string userId);

    Task<IResult> UpdateEmailAsync(UpdateEmailRequest request);

    Task<IResult> ChangePasswordAsync(ChangePasswordRequest request);
    Task<IResult> ChangePasswordOtherUserAsync(ChangePasswordOtherUserRequest request);

}