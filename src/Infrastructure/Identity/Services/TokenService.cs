using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
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
using MyReliableSite.Domain.Constants;
using MyReliableSite.Domain.Identity;
using MyReliableSite.Infrastructure.Identity.Models;
using MyReliableSite.Infrastructure.Persistence.Contexts;
using MyReliableSite.Shared.DTOs.EmailTemplates;
using MyReliableSite.Shared.DTOs.General.Requests;
using MyReliableSite.Shared.DTOs.Identity;
using MyReliableSite.Shared.DTOs.MFA;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static MyReliableSite.Domain.Constants.PermissionConstants;

namespace MyReliableSite.Infrastructure.Identity.Services;

public class TokenService : ITokenService
{
    private readonly TenantManagementDbContext _tenantContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<TokenService> _localizer;
    private readonly MailSettings _mailSettings;
    private readonly JwtSettings _config;
    private readonly ITenantService _tenantService;
    private readonly ICurrentUser _user;
    private readonly IMFAuthenticatorService _mFAuthenticatorService;
    private readonly ISettingService _settingService;
    private readonly IUserLoginHistoryService _userLoginHistoryService;
    private readonly IMailService _mailService;
    private readonly IJobService _jobService;
    private readonly IEmailTemplateService _templateService;
    public TokenService(
        UserManager<ApplicationUser> userManager,
        IOptions<JwtSettings> config,
        IStringLocalizer<TokenService> localizer,
        IOptions<MailSettings> mailSettings,
        ITenantService tenantService,
        TenantManagementDbContext tenantContext,
        ICurrentUser user,
        IMFAuthenticatorService mFAuthenticatorService,
        ISettingService settingService,
        IUserLoginHistoryService userLoginHistoryService,
        IMailService mailService,
        IJobService jobService,
        IEmailTemplateService templateService)
    {
        _userManager = userManager;
        _localizer = localizer;
        _mailSettings = mailSettings.Value;
        _config = config.Value;
        _tenantService = tenantService;
        _tenantContext = tenantContext;
        _user = user;
        _mFAuthenticatorService = mFAuthenticatorService;
        _settingService = settingService;
        _userLoginHistoryService = userLoginHistoryService;
        _mailService = mailService;
        _jobService = jobService;
        _templateService = templateService;
    }

    public async Task<IResult<TokenResponse>> GetTokenAsync(TokenRequest request, string ipAddress, string deviceName, string location, bool flgAdmin, string origin)
    {
        ApplicationUser user = null;
        if (flgAdmin)
        {
            if (string.IsNullOrEmpty(request.UserName))
            {
                throw new IdentityException(_localizer["identity.usernotfound"], statusCode: HttpStatusCode.Unauthorized);
            }

            user = await _userManager.FindByNameAsync(request.UserName.Trim());
        }
        else
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                throw new IdentityException(_localizer["identity.usernotfound"], statusCode: HttpStatusCode.Unauthorized);
            }

            user = await _userManager.FindByEmailAsync(request.Email.Trim());
        }

        if (user == null)
        {
            throw new IdentityException(_localizer["identity.usernotfound"], statusCode: HttpStatusCode.Unauthorized);
        }

        bool isallowed = false;
        var roles = await _userManager.GetRolesAsync(user);
        if (flgAdmin)
        {
            isallowed = roles.Contains(RoleConstants.Admin) || await _userManager.IsInRoleAsync(user, RoleConstants.SuperAdmin);

            // await _userManager.IsInRoleAsync(user, RoleConstants.Admin) || await _userManager.IsInRoleAsync(user, RoleConstants.SuperAdmin);
        }
        else
        {
            isallowed = isallowed = roles.Contains(RoleConstants.Client);

            // await _userManager.IsInRoleAsync(user, RoleConstants.Client);
        }

        /*if (user.UserRestrictedIps.Count > 0)
        {
            isallowed = user.UserRestrictedIps.Any(m => m.RestrictAccessIPAddress == ipAddress);
        }*/

        if (!isallowed)
        {
            throw new IdentityException(_localizer["identity.userrolenotfound"], statusCode: HttpStatusCode.Unauthorized);
        }

        string tenant = user.Tenant;
        var tenantInfo = await _tenantContext.Tenants.Where(a => a.Key == tenant).FirstOrDefaultAsync();
        if (tenant != MultitenancyConstants.Root.Key)
        {
            if (!tenantInfo.IsActive)
            {
                throw new InvalidTenantException(_localizer["tenant.inactive"]);
            }

            if (DateTime.UtcNow > tenantInfo.ValidUpto)
            {
                throw new InvalidTenantException(_localizer["tenant.expired"]);
            }
        }

        if (!user.IsActive)
        {
            throw new IdentityException(_localizer["identity.usernotactive"], statusCode: HttpStatusCode.Unauthorized);
        }

        // todo mfa

        var tenantSettings = await _settingService.GetSettingDetailsAsync(user.Tenant);

        if (tenantSettings != null)
        {

            bool mFAEnable = flgAdmin ? tenantSettings.Data.EnableAdminMFA : tenantSettings.Data.EnableClientMFA;

            if (user.TwoFactorEnabled && !request.TrustDevice && mFAEnable)
            {
                EnableAuthenticatorRequest enableAuthenticatorRequest = new EnableAuthenticatorRequest();
                enableAuthenticatorRequest.UserId = user.Id;
                if (await _userManager.GetAuthenticatorKeyAsync(user) != null && user.MFARequest)
                {
                    return await Result<TokenResponse>.SuccessAsync(new List<string>()
            {
                "User has authenticator app as MFA.",
                user.Id,
                user.Email,
                user.UserName,
                "true"
            });
                }

                Result<string> otpResponse = await _mFAuthenticatorService.GenerateOTPEmail(enableAuthenticatorRequest);
                return await Result<TokenResponse>.SuccessAsync(new List<string>()
            {
                "TwoFactorEnable is true for that user!" + otpResponse.Messages[0] + " Please login through OTP.",
                user.Id,
                user.Email,
                user.UserName,
                "false"
            });
            }
        }

        // if pending MFA oR OTP
        /*if (user.MFARequest)
        {
            throw new IdentityException(_localizer["identity.USER HAS ENABLED MFA"], statusCode: HttpStatusCode.Unauthorized);
        }*/

        // if (string.IsNullOrEmpty(user.OTPCode))
        // {
        //    throw new IdentityException(_localizer["identity.USER HAS not provided OTP Code"], statusCode: HttpStatusCode.Unauthorized);
        // }

        bool passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            await _userLoginHistoryService.CreateUserLoginHistoryAsync(new CreateUserLoginHistoryRequest()
            {
                DeviceName = deviceName,
                IpAddress = ipAddress,
                Location = location,
                LoginTime = DateTime.UtcNow,
                Status = Shared.DTOs.Identity.UserLoginStatus.ERR,
                UserId = user.Id
            });
            throw new IdentityException(_localizer["identity.invalidcredentials"], statusCode: HttpStatusCode.Unauthorized);
        }

        if (_mailSettings.EnableVerification && !user.EmailConfirmed)
        {
            await ResendConfirmationEmail(user, origin);
            throw new IdentityException(_localizer["identity.emailnotconfirmed"], statusCode: HttpStatusCode.Unauthorized);
        }

        user.LastLoggedIn = DateTime.UtcNow;
        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_config.RefreshTokenExpirationInDays);
        await _userManager.UpdateAsync(user);
        string token = await GenerateJwtAsync(user, ipAddress, flgAdmin);
        var response = new TokenResponse(token, user.RefreshToken, user.RefreshTokenExpiryTime);
        await _userLoginHistoryService.CreateUserLoginHistoryAsync(new CreateUserLoginHistoryRequest()
        {
            DeviceName = deviceName,
            IpAddress = ipAddress,
            Location = location,
            LoginTime = DateTime.UtcNow,
            Status = Shared.DTOs.Identity.UserLoginStatus.OK,
            UserId = user.Id
        });
        return await Result<TokenResponse>.SuccessAsync(response);
    }

    public async Task<TokenResponse> GetUserUpdatedToken(string ipAddress, bool flgAdmin, string userId)
    {

        ApplicationUser user = await _userManager.FindByIdAsync(userId);
        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_config.RefreshTokenExpirationInDays);
        await _userManager.UpdateAsync(user);
        string token = await GenerateJwtAsync(user, ipAddress, flgAdmin);
        var response = new TokenResponse(token, user.RefreshToken, user.RefreshTokenExpiryTime);
        return response;
    }

    public async Task<IResult<TokenResponse>> GetTokenByOTPAsync(OTPRequest request, string ipAddress, string deviceName, string location, bool flgAdmin)
    {
        ApplicationUser user;
        if (flgAdmin)
        {
            user = await _userManager.FindByNameAsync(request.UserName.Trim());
        }
        else
        {
            user = await _userManager.FindByEmailAsync(request.Email.Trim());
        }

        if (user == null)
        {
            throw new IdentityException(_localizer["identity.usernotfound"], statusCode: HttpStatusCode.NotFound);
        }

        bool isallowed;
        if (flgAdmin)
        {
            isallowed = await _userManager.IsInRoleAsync(user, RoleConstants.Admin) || await _userManager.IsInRoleAsync(user, RoleConstants.SuperAdmin);
        }
        else
        {
            isallowed = await _userManager.IsInRoleAsync(user, RoleConstants.Client);
        }

        if (!isallowed)
        {
            throw new IdentityException(_localizer["identity.usernotfound"], statusCode: HttpStatusCode.Unauthorized);
        }

        if (string.IsNullOrEmpty(request.OTPCode) || user.OTPCode != request.OTPCode)
        {
            throw new IdentityException(_localizer["OTP Code does not match!"], statusCode: HttpStatusCode.NotFound);
        }

        user.OTPCode = null;
        await _userManager.UpdateAsync(user);
        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_config.RefreshTokenExpirationInDays);
        await _userManager.UpdateAsync(user);
        string token = await GenerateJwtAsync(user, ipAddress, flgAdmin);
        var response = new TokenResponse(token, user.RefreshToken, user.RefreshTokenExpiryTime);
        await _userLoginHistoryService.CreateUserLoginHistoryAsync(new CreateUserLoginHistoryRequest()
        {
            DeviceName = deviceName,
            IpAddress = ipAddress,
            Location = location,
            LoginTime = DateTime.UtcNow,
            Status = Shared.DTOs.Identity.UserLoginStatus.OK,
            UserId = user.Id
        });
        return await Result<TokenResponse>.SuccessAsync(response);
    }

    public async Task<IResult<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress, bool flgAdmin)
    {
        if (request is null)
        {
            throw new IdentityException(_localizer["identity.invalidtoken"], statusCode: HttpStatusCode.Unauthorized);
        }

        var userPrincipal = GetPrincipalFromExpiredToken(request.Token);
        string userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            throw new IdentityException(_localizer["identity.usernotfound"], statusCode: HttpStatusCode.NotFound);
        }

        bool isallowed;
        if (flgAdmin)
        {
            isallowed = await _userManager.IsInRoleAsync(user, RoleConstants.Admin) || await _userManager.IsInRoleAsync(user, RoleConstants.SuperAdmin);
        }
        else
        {
            isallowed = await _userManager.IsInRoleAsync(user, RoleConstants.Client);
        }

        if (!isallowed)
        {
            throw new IdentityException(_localizer["identity.usernotfound"], statusCode: HttpStatusCode.Unauthorized);
        }

        if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new IdentityException(_localizer["identity.invalidtoken"], statusCode: HttpStatusCode.Unauthorized);
        }

        string token = await GenerateEncryptedTokenAsync(GetSigningCredentials(), GetClaims(user, ipAddress), user, flgAdmin);
        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_config.RefreshTokenExpirationInDays);
        await _userManager.UpdateAsync(user);
        var response = new TokenResponse(token, user.RefreshToken, user.RefreshTokenExpiryTime);
        return await Result<TokenResponse>.SuccessAsync(response);
    }

    public async Task<IResult<TokenResponse>> LoginAdminAsClientAsync(string clientId, string ipAddress)
    {
        string currentAdminUser = _user.GetUserId().ToString();
        bool isAuthenticated = _user.IsAuthenticated();
        if (!isAuthenticated)
        {
            throw new IdentityException(_localizer["CurrentUser.UnAuthenticated"], statusCode: HttpStatusCode.Unauthorized);
        }

        var user = await _userManager.FindByIdAsync(clientId.Trim());
        if (user == null)
        {
            throw new IdentityException(_localizer["client.usernotfound"], statusCode: HttpStatusCode.Unauthorized);
        }

        string tenant = user.Tenant;
        var tenantInfo = await _tenantContext.Tenants.Where(a => a.Key == tenant).FirstOrDefaultAsync();
        if (tenant != MultitenancyConstants.Root.Key)
        {
            if (!tenantInfo.IsActive)
            {
                throw new InvalidTenantException(_localizer["tenant.inactive"]);
            }

            if (DateTime.UtcNow > tenantInfo.ValidUpto)
            {
                throw new InvalidTenantException(_localizer["tenant.expired"]);
            }
        }

        if (!user.IsActive)
        {
            throw new IdentityException(_localizer["identity.usernotactive"], statusCode: HttpStatusCode.Unauthorized);
        }

        if (user.TwoFactorEnabled)
        {
            // throw new IdentityException(_localizer["identity.USER HAS pending two factor"], statusCode: HttpStatusCode.Unauthorized);
        }

        // if pending MFA oR OTP

        // if (user.MFARequest)

        // {

        // throw new IdentityException(_localizer["identity.USER HAS ENABLED MFA"], statusCode: HttpStatusCode.Unauthorized);

        // }

        // if (string.IsNullOrEmpty(user.OTPCode))

        // {

        // throw new IdentityException(_localizer["identity.USER HAS not provided OTP Code"], statusCode: HttpStatusCode.Unauthorized);

        // }

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_config.RefreshTokenExpirationInDays);
        await _userManager.UpdateAsync(user);
        string token = await GenerateJwtAsync(user, ipAddress, true);
        var response = new TokenResponse(token, user.RefreshToken, user.RefreshTokenExpiryTime, true, currentAdminUser);

        return await Result<TokenResponse>.SuccessAsync(response);

    }

    public async Task<IResult<TokenResponse>> LoginClientAsAdminAsync(string adminId, string ipAddress)
    {
        bool isAuthenticated = _user.IsAuthenticated();
        if (!isAuthenticated)
        {
            throw new IdentityException(_localizer["CurrentUser.UnAuthenticated"], statusCode: HttpStatusCode.Unauthorized);
        }

        var user = await _userManager.FindByIdAsync(adminId.Trim());
        if (user == null)
        {
            throw new IdentityException(_localizer["admin.usernotfound"], statusCode: HttpStatusCode.Unauthorized);
        }

        string tenant = user.Tenant;
        var tenantInfo = await _tenantContext.Tenants.Where(a => a.Key == tenant).FirstOrDefaultAsync();
        if (tenant != MultitenancyConstants.Root.Key)
        {
            if (!tenantInfo.IsActive)
            {
                throw new InvalidTenantException(_localizer["tenant.inactive"]);
            }

            if (DateTime.UtcNow > tenantInfo.ValidUpto)
            {
                throw new InvalidTenantException(_localizer["tenant.expired"]);
            }
        }

        if (!user.IsActive)
        {
            throw new IdentityException(_localizer["identity.usernotactive"], statusCode: HttpStatusCode.Unauthorized);
        }

        if (user.TwoFactorEnabled)
        {
            // throw new IdentityException(_localizer["identity.USER HAS pending two factor"], statusCode: HttpStatusCode.Unauthorized);
        }

        // if pending MFA oR OTP

        // if (user.MFARequest)

        // {

        // throw new IdentityException(_localizer["identity.USER HAS ENABLED MFA"], statusCode: HttpStatusCode.Unauthorized);

        // }

        // if (string.IsNullOrEmpty(user.OTPCode))

        // {

        // throw new IdentityException(_localizer["identity.USER HAS not provided OTP Code"], statusCode: HttpStatusCode.Unauthorized);

        // }

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_config.RefreshTokenExpirationInDays);
        await _userManager.UpdateAsync(user);
        string token = await GenerateJwtAsync(user, ipAddress, true);
        var response = new TokenResponse(token, user.RefreshToken, user.RefreshTokenExpiryTime);

        return await Result<TokenResponse>.SuccessAsync(response);

    }

    public async Task<IResult> RevokeToken(string userId)
    {
        var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

        // return false if no user found with token
        if (user == null)
            throw new IdentityException(_localizer["identity.usernotfound"], statusCode: HttpStatusCode.Unauthorized);

        user.RefreshTokenExpiryTime = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return await Result.SuccessAsync();
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

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Key)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new IdentityException(_localizer["identity.invalidtoken"], statusCode: HttpStatusCode.Unauthorized);
        }

        return principal;
    }

    private SigningCredentials GetSigningCredentials()
    {
        byte[] secret = Encoding.UTF8.GetBytes(_config.Key);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }

    private async Task ResendConfirmationEmail(ApplicationUser user, string origin)
    {
        string emailVerificationUri = await GetEmailVerificationUriAsync(user, origin);
        var userDto = user.Adapt<UserDetailsDto>();

        EmailTemplateDto emailDto = await _templateService.GenerateEmailConfirmationMail(userDto, emailVerificationUri);
        var mailRequest = new MailRequest
        {
            From = _mailSettings.From,
            To = new List<string> { user.Email },
            Body = emailDto.Body,
            Subject = emailDto.Subject
        };
        _jobService.Enqueue(() => _mailService.SendAsync(mailRequest));

    }

    private async Task<string> GetEmailVerificationUriAsync(ApplicationUser user, string origin)
    {
        string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        origin = await GetDomainApiUriAsync(user, origin);

        const string route = "verify-email";
        var endpointUri = new Uri(string.Concat($"{origin}/", $"{route}/", $"{user.Id}"));
        string verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "code", code);
        verificationUri = QueryHelpers.AddQueryString(verificationUri, "tenant", user.Tenant);

        if (!string.IsNullOrEmpty(user.BrandId))
        {
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "brandId", user.BrandId);
        }

        return verificationUri;
    }

    private async Task<string> GetDomainApiUriAsync(ApplicationUser user, string origin)
    {
        if (await _userManager.IsInRoleAsync(user, RoleConstants.Admin))
        {
            origin = string.Concat($"{origin}/", "admin");
        }
        else if (await _userManager.IsInRoleAsync(user, RoleConstants.Client))
        {
            origin = string.Concat($"{origin}/", "client");
        }

        return origin;
    }
}
