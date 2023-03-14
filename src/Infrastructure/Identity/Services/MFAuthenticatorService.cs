using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Identity.Exceptions;
using MyReliableSite.Application.Identity.Interfaces;
using MyReliableSite.Application.Multitenancy;
using MyReliableSite.Application.Settings;
using MyReliableSite.Application.Settings.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Infrastructure.Identity.Models;
using MyReliableSite.Shared.DTOs.EmailTemplates;
using MyReliableSite.Shared.DTOs.General.Requests;
using MyReliableSite.Shared.DTOs.Identity;
using MyReliableSite.Shared.DTOs.MFA;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

namespace MyReliableSite.Infrastructure.Identity.Services;

public class MFAuthenticatorService : IMFAuthenticatorService
{
    private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<MFAuthenticatorService> _logger;
    private readonly IStringLocalizer<MFAuthenticatorService> _localizer;
    private readonly UrlEncoder _urlEncoder;
    private readonly IJobService _jobService;
    private readonly IMailService _mailService;
    private readonly MailSettings _mailSettings;
    private readonly ITenantService _tenantService;
    private readonly JwtSettings _config;
    private readonly ISettingService _settingService;
    private readonly IEmailTemplateService _templateService;

    public MFAuthenticatorService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<MFAuthenticatorService> logger,
        IStringLocalizer<MFAuthenticatorService> localizer,
        UrlEncoder urlEncoder,
        IJobService jobService,
        IMailService mailService,
        ITenantService tenantService,
        IEmailTemplateService templateService,
        IOptions<JwtSettings> config,
        IOptions<MailSettings> mailSettings,
        ISettingService settingService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _localizer = localizer;
        _urlEncoder = urlEncoder;
        _jobService = jobService;
        _mailService = mailService;
        _mailSettings = mailSettings.Value;
        _tenantService = tenantService;
        _templateService = templateService;
        _config = config.Value;
        _settingService = settingService;
    }

    public async Task<EnableAuthenticatorResponse> GETqRCodeUriForAuthenticatorApp(EnableAuthenticatorRequest enableAuthenticatorRequest)
    {
        var user = await _userManager.FindByIdAsync(enableAuthenticatorRequest.UserId);
        if (user == null)
        {
            throw new IdentityException(_localizer["identity.usernotfound"], statusCode: HttpStatusCode.NotFound);
        }

        return await LoadSharedKeyAndQrCodeUriAsync(user);
    }

    public async Task<EnableAuthenticatorResponse> ValidateMFACodeAndAddApp(EnableAuthenticatorRequest enableAuthenticatorRequest, bool flgAdmin, string iPaddress)
    {
        EnableAuthenticatorResponse responseModel = new EnableAuthenticatorResponse();
        var user = await _userManager.FindByIdAsync(enableAuthenticatorRequest.UserId);
        if (user == null)
        {
            throw new IdentityException(_localizer["identity.usernotfound"], statusCode: HttpStatusCode.Unauthorized);
        }

        if (string.IsNullOrEmpty(enableAuthenticatorRequest.Code))
        {
            return await LoadSharedKeyAndQrCodeUriAsync(user);

        }

        // Strip spaces and hypens
        string verificationCode = enableAuthenticatorRequest.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

        bool is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
            user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

        if (enableAuthenticatorRequest.isRemember)
        await _signInManager.RememberTwoFactorClientAsync(user);
        /*
        await _signInManager.IsTwoFactorClientRememberedAsync(user);*/

        if (!is2faTokenValid)
        {
            await LoadSharedKeyAndQrCodeUriAsync(user);

            throw new IdentityException(_localizer["identity.Verification code is invalid"], statusCode: HttpStatusCode.Unauthorized);
        }
        else
        {
             responseModel.tokenResponse = await GetUserUpdatedToken(iPaddress, flgAdmin, user.Id);
        }

        await _userManager.SetTwoFactorEnabledAsync(user, true);

        string userId = await _userManager.GetUserIdAsync(user);
        user.MFARequest = true;
        await _userManager.UpdateAsync(user);
        _logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", userId);

        responseModel.StatusMessage = "Your authenticator app has been verified.";

        if (await _userManager.CountRecoveryCodesAsync(user) == 0)
        {
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            responseModel.RecoveryCodes = recoveryCodes.ToArray();

        }

        return responseModel;
    }

    #region private methods
    private async Task<TokenResponse> GetUserUpdatedToken(string ipAddress, bool flgAdmin, string userId)
    {

        ApplicationUser user = await _userManager.FindByIdAsync(userId);
        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_config.RefreshTokenExpirationInDays);
        await _userManager.UpdateAsync(user);
        string token = await GenerateJwtAsync(user, ipAddress, flgAdmin);
        var response = new TokenResponse(token, user.RefreshToken, user.RefreshTokenExpiryTime);
        return response;
    }

    private async Task<string> GenerateJwtAsync(ApplicationUser user, string ipAddress, bool flgAdmin)
    {
        return await GenerateEncryptedTokenAsync(GetSigningCredentials(), GetClaims(user, ipAddress), user, flgAdmin);
    }

    private IEnumerable<Claim> GetClaims(ApplicationUser user, string ipAddress)
    {
        string tenant = _tenantService.GetCurrentTenant()?.Key;
        return new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email),
                new("fullName", $"{user.FullName}"),
                new(ClaimTypes.Name, user.FullName),
                new(ClaimTypes.Surname, user.FullName),
                new("ipAddress", ipAddress),
                new("tenant", tenant)
            };
    }

    private string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private async Task<string> GenerateEncryptedTokenAsync(SigningCredentials signingCredentials, IEnumerable<Claim> claims, ApplicationUser user, bool flgAdmin)
    {
        DateTime expiryTime = DateTime.UtcNow.AddMinutes(_config.TokenExpirationInMinutes);

        var tenantSettings = await _settingService.GetSettingDetailsAsync(user.Tenant);

        if (tenantSettings != null)
        {
            if (flgAdmin)
            {
                if (tenantSettings.Data.defaultInactivityMinutesLockAdmin > 0)
                {
                    expiryTime = DateTime.UtcNow.AddMinutes(tenantSettings.Data.defaultInactivityMinutesLockAdmin);
                }
            }
            else
            {
                if (tenantSettings.Data.defaultInactivityMinutesLockClient > 0)
                {
                    expiryTime = DateTime.UtcNow.AddMinutes(tenantSettings.Data.defaultInactivityMinutesLockClient);
                }
            }
        }

        var token = new JwtSecurityToken(
           claims: claims,
           expires: expiryTime,
           signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();

        return tokenHandler.WriteToken(token);
    }

    private SigningCredentials GetSigningCredentials()
    {
        byte[] secret = Encoding.UTF8.GetBytes(_config.Key);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }
    #endregion
    public async Task<UserAuthenticatorStatus> GetCurrentStatusOfTwoFactorAuthentication(EnableAuthenticatorRequest enableAuthenticatorRequest)
    {
        var user = await _userManager.FindByIdAsync(enableAuthenticatorRequest.UserId);
        if (user == null)
        {
            throw new IdentityException(_localizer["identity.usernotfound"], statusCode: HttpStatusCode.NotFound);
        }

        return new UserAuthenticatorStatus()
        {
            HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
            HasAppAuthenticator = user.MFARequest,
            Is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
            IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
            RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user)
        };
    }

    public async Task<bool> RemoveTwoFactorAuthentication(EnableAuthenticatorRequest enableAuthenticatorRequest)
    {
        var user = await _userManager.FindByIdAsync(enableAuthenticatorRequest.UserId);
        if (user == null)
        {
            throw new IdentityException(_localizer["identity.usernotfound"], statusCode: HttpStatusCode.NotFound);
        }

        await _signInManager.ForgetTwoFactorClientAsync();
        return true;
    }

    public async Task<UserAuthenticatorStatus> ResetAuthenticator(EnableAuthenticatorRequest enableAuthenticatorRequest)
    {
        var user = await _userManager.FindByIdAsync(enableAuthenticatorRequest.UserId);
        if (user == null)
        {
            throw new IdentityException(_localizer["identity.usernotfound"], statusCode: HttpStatusCode.NotFound);
        }

        await _userManager.SetTwoFactorEnabledAsync(user, false);
        await _userManager.ResetAuthenticatorKeyAsync(user);
        _logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", user.Id);
        user.MFARequest = false;
        await _userManager.UpdateAsync(user);
        await _signInManager.RefreshSignInAsync(user);

        // "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.";

        return new UserAuthenticatorStatus()
        {
            HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
            HasAppAuthenticator = user.MFARequest,
            Is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
            IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
            RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user)
        };
    }

    public async Task<Result<string>> EnableDisable2fa(EnableDisableAuthenticatorRequest enableAuthenticatorRequest)
    {
        var user = await _userManager.FindByIdAsync(enableAuthenticatorRequest.UserId);
        if (user == null)
        {
            throw new IdentityException(_localizer["identity.usernotfound"], statusCode: HttpStatusCode.NotFound);
        }

        var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, enableAuthenticatorRequest.Flag);
        if (!disable2faResult.Succeeded)
        {
            throw new InvalidOperationException($"Unexpected error occurred disabling 2FA for user with ID '{user.Id}'.");
        }

        string message = string.Empty;
        if (enableAuthenticatorRequest.Flag)
        {
            _logger.LogInformation("User with ID '{UserId}' has enabled 2fa.", user.Id);
            message = "User set enabled 2fa.";
        }
        else
        {
            _logger.LogInformation("User with ID '{UserId}' has disabled 2fa.", user.Id);
            message = "User set disabled 2fa.";
        }

        // "2fa has been disabled. You can reenable 2fa when you setup an authenticator app";

        return await Result<string>.SuccessAsync(message);
    }

    private async Task<EnableAuthenticatorResponse> LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user)
    {
        // Load the authenticator key & QR code URI to display on the form
        string unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        }

        string email = await _userManager.GetEmailAsync(user);

        return new EnableAuthenticatorResponse()
        {
            AuthenticatorUri = GenerateQrCodeUri(email, unformattedKey),
            SharedKey = FormatKey(unformattedKey)
        };
    }

    public async Task<string[]> ResetGenerateRecoveryCodes(EnableAuthenticatorRequest enableAuthenticatorRequest)
    {
        var user = await _userManager.FindByIdAsync(enableAuthenticatorRequest.UserId);
        if (user == null)
        {
            throw new IdentityException(_localizer["identity.usernotfound"], statusCode: HttpStatusCode.NotFound);
        }

        bool isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
        string userId = await _userManager.GetUserIdAsync(user);
        if (!isTwoFactorEnabled)
        {
            throw new InvalidOperationException($"Cannot generate recovery codes for user with ID '{userId}' as they do not have 2FA enabled.");
        }

        var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

        _logger.LogInformation("User with ID '{UserId}' has generated new 2FA recovery codes.", userId);
        return recoveryCodes.ToArray();

        // StatusMessage = "You have generated new recovery codes.";
    }

    public async Task<Result<string>> Validate2FAEnable2FAByEmail(EnableAuthenticatorRequest enableAuthenticatorRequest)
    {
        var user = await _userManager.FindByIdAsync(enableAuthenticatorRequest.UserId);
        if (user == null)
        {
            throw new IdentityException(_localizer["identity.usernotfound"], statusCode: HttpStatusCode.NotFound);
        }

        if (user.OTPCode != enableAuthenticatorRequest.Code)
        {
            throw new IdentityException(_localizer["identity.otpcodeisinvalid"], statusCode: HttpStatusCode.NotFound);
        }

        var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, true);
        if (!disable2faResult.Succeeded)
        {
            throw new InvalidOperationException($"Unexpected error occurred disabling 2FA for user with ID '{user.Id}'.");
        }

        user.OTPCode = null;
        await _userManager.UpdateAsync(user);

        return await Result<string>.SuccessAsync($"Email 2FA has enabled for your email {user.Email}!");
    }

    public async Task<Result<string>> GenerateOTPEmail(EnableAuthenticatorRequest enableAuthenticatorRequest)
    {
        var user = await _userManager.FindByIdAsync(enableAuthenticatorRequest.UserId);
        if (user == null)
        {
            throw new IdentityException(_localizer["identity.usernotfound"], statusCode: HttpStatusCode.NotFound);
        }

        var messages = new List<string> { string.Format(_localizer["Admin User {0} Registered."], user.UserName) };
        if (_mailSettings.EnableVerification)
        {
            // send verification email
            // string emailVerificationUri = await GetEmailVerificationUriAsync(user);

            int otpcode = getSixDigitCode();
            user.OTPCode = otpcode.ToString();
            await _userManager.UpdateAsync(user);
            EmailTemplateDto emailDto = await _templateService.GenerateEmailOTP(user.Adapt<UserDetailsDto>(), otpcode.ToString());
            var mailRequest = new MailRequest
            {
                From = _mailSettings.From,
                To = new List<string> { user.Email },
                Body = emailDto.Body,
                Subject = emailDto.Subject
            };
            _jobService.Enqueue(() => _mailService.SendAsync(mailRequest));
            messages.Add(_localizer[$"Please check {user.Email} to verify your OTP!"]);
        }

        return await Result<string>.SuccessAsync($"Please check email {user.Email} to verify your OTP!");
    }

    // This is how you can generate six digits random code
    private int getSixDigitCode()
    {
        Random random = new Random();
        int aftCode = random.Next(100001, 999999);
        return aftCode;
    }

    private string FormatKey(string unformattedKey)
    {
        var result = new StringBuilder();
        int currentPosition = 0;
        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
            currentPosition += 4;
        }

        if (currentPosition < unformattedKey.Length)
        {
            result.Append(unformattedKey.Substring(currentPosition));
        }

        return result.ToString().ToLowerInvariant();
    }

    private string GenerateQrCodeUri(string email, string unformattedKey)
    {
        return string.Format(
            AuthenticatorUriFormat,
            _urlEncoder.Encode("M2M.Paul"),
            _urlEncoder.Encode(email),
            unformattedKey);
    }
}
