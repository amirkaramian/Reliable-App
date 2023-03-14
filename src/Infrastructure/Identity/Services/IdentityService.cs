using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Identity.Exceptions;
using MyReliableSite.Application.Identity.Interfaces;
using MyReliableSite.Application.ManageModule.Interfaces;
using MyReliableSite.Application.Multitenancy;
using MyReliableSite.Application.Settings;
using MyReliableSite.Application.Storage;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Domain.Identity;
using MyReliableSite.Domain.ManageModule;
using MyReliableSite.Domain.Tickets;
using MyReliableSite.Infrastructure.Identity.Models;
using MyReliableSite.Shared.DTOs.EmailTemplates;
using MyReliableSite.Shared.DTOs.General.Requests;
using MyReliableSite.Shared.DTOs.Identity;
using MyReliableSite.Shared.DTOs.Notifications;
using MyReliableSite.Shared.DTOs.Notifications.Enums;
using MyReliableSite.Shared.DTOs.ReCaptcha;
using Newtonsoft.Json;
using System.Text;

namespace MyReliableSite.Infrastructure.Identity.Services;

public class IdentityService : IIdentityService
{
    private readonly IFileStorageService _fileStorage;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJobService _jobService;
    private readonly IMailService _mailService;
    private readonly MailSettings _mailSettings;
    private readonly IStringLocalizer<IdentityService> _localizer;
    private readonly ITenantService _tenantService;
    private readonly ICurrentUser _user;
    private readonly ITokenService _tokenService;
    private readonly IEmailTemplateService _templateService;
    private readonly GoogleSettings _googleSettings;
    private readonly INotificationService _notificationService;
    private readonly IRepositoryAsync _repo;
    private readonly IAdminGroupModuleManagementService _adminGroupModuleManagementService;
    private readonly IUserModuleManagementService _userModuleManagementService;

    private readonly RoleManager<ApplicationRole> _roleManager;
    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        IJobService jobService,
        IMailService mailService,
        IOptions<MailSettings> mailSettings,
        IStringLocalizer<IdentityService> localizer,
        ITenantService tenantService,
        IFileStorageService fileStorage,
        IEmailTemplateService templateService,
        ICurrentUser user,
        IOptions<GoogleSettings> googleSettings,
        ITokenService tokenService,
        IAdminGroupModuleManagementService adminGroupModuleManagementService,
        IUserModuleManagementService userModuleManagementService,
        INotificationService notificationService,
        IRepositoryAsync repo)
    {
        _userManager = userManager;
        _jobService = jobService;
        _mailService = mailService;
        _mailSettings = mailSettings.Value;
        _localizer = localizer;
        _tenantService = tenantService;
        _signInManager = signInManager;
        _fileStorage = fileStorage;
        _templateService = templateService;
        _user = user;
        _tokenService = tokenService;
        _googleSettings = googleSettings.Value;
        _notificationService = notificationService;
        _repo = repo;
        _adminGroupModuleManagementService = adminGroupModuleManagementService;
        _userModuleManagementService = userModuleManagementService;
        _roleManager = roleManager;
    }

    public IdentityService()
    {
    }

    public async Task<IResult> verifyRecaptchaAsync(ReCaptchaClientRequest reCaptchaClientRequest)
    {

        var dictionary = new Dictionary<string, string>
                    {
                        { "secret", _googleSettings.RecaptchaSecretKey },
                        { "response", reCaptchaClientRequest.ReCaptchaToken }
                    };

        var postContent = new FormUrlEncodedContent(dictionary);

        HttpResponseMessage recaptchaResponse = null;
        string stringContent = string.Empty;

        // Call recaptcha api and validate the token
        using (var http = new HttpClient())
        {
            recaptchaResponse = await http.PostAsync("https://www.google.com/recaptcha/api/siteverify", postContent);
            stringContent = await recaptchaResponse.Content.ReadAsStringAsync();
        }

        if (!recaptchaResponse.IsSuccessStatusCode)
        {
            return await Result<string>.FailAsync("Unable to verify recaptcha token");
        }

        if (string.IsNullOrEmpty(stringContent))
        {
            return await Result<string>.FailAsync("Invalid reCAPTCHA verification response");
        }

        var googleReCaptchaResponse = JsonConvert.DeserializeObject<ReCaptchaResponse>(stringContent);

        if (!googleReCaptchaResponse.Success)
        {
            string errors = string.Join(",", googleReCaptchaResponse.ErrorCodes);

            return await Result<string>.FailAsync(errors);
        }

        // Captcha was success , let's check the score, in our case, for example, anything less than 0.5 is considered as a bot user which we would not allow ...
        // the passing score might be higher or lower according to the sensitivity of your action

        if (googleReCaptchaResponse.Score < 0.5)
        {
            return await Result<string>.FailAsync("This is a potential bot. Signup request rejected");
        }

        // TODO: Continue with doing the actual signup process, since now we know the request was done by potentially really human

        return await Result<string>.SuccessAsync();
    }

    public async Task<IResult> RegisterAdminAsync(RegisterAdminRequest request, string origin)
    {
        var users = await _userManager.Users.ToListAsync();
        var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
        if (userWithSameUserName != null)
        {
            throw new IdentityException(string.Format(_localizer["Username {0} is already taken."], request.UserName));
        }

        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.UserName,
            FullName = request.FullName,
            Status = request.Status,
            IsActive = true,
            Tenant = RoleConstants.Admin,
            AdminGroupId = request.AdminGroupId,
            CreatedOn = DateTime.UtcNow
        };
        if (request.IpAddresses?.Count > 0)
        {
            foreach (string ipAddress in request.IpAddresses.Distinct())
            {
                var restrictIPId = await _repo.CreateAsync<UserRestrictedIp>(new UserRestrictedIp() { RestrictAccessIPAddress = ipAddress, UserId = user.Id });
                await _repo.SaveChangesAsync();
            }
        }

        var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
        if (userWithSameEmail == null)
        {
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                try
                {
                    await _userManager.AddToRoleAsync(user, RoleConstants.Admin);
                }
                catch
                {
                }

                var messages = new List<string> { string.Format(_localizer["Admin User {0} Registered."], user.UserName) };
                if (_mailSettings.EnableVerification)
                {
                    // send verification email
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
                    messages.Add(_localizer[$"Please check {user.Email} to verify your account!"]);
                }

                if (!string.IsNullOrEmpty(request.AdminGroupId))
                {
                    _jobService.Enqueue(() => BindUserModulesOnRegisteration(request, user));

                }

                return await Result<string>.SuccessAsync(user.Id, messages: messages);
            }
            else
            {
                throw new IdentityException(_localizer["Validation Errors Occurred."], result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
            }
        }
        else
        {
            throw new IdentityException(string.Format(_localizer["Email {0} is already registered."], request.Email));
        }
    }

    public async Task BindUserModulesOnRegisteration(RegisterAdminRequest request, ApplicationUser user)
    {
        var adminGroup = await _adminGroupModuleManagementService.GetAdminGroupModuleManagementByAdminGroupIdAsync(request.AdminGroupId);
        if (adminGroup != null)
        {

            var adminGroupListDto = adminGroup.Data;
            foreach (var item in adminGroupListDto)
            {
                await _userModuleManagementService.CreateUserModuleManagementAsync(
               new Shared.DTOs.ManageModule.CreateUserModuleManagementRequest()
               {
                   Name = item.Name,
                   IsActive = true,
                   PermissionDetail = item.PermissionDetail,
                   UserId = user.Id,
                   Tenant = user.Tenant

               });
            }
        }
    }

    public async Task BindClientModules(ApplicationUser user)
    {
        var modules = await _repo.FindByConditionAsync<Module>(x => x.Tenant.ToLower() == user.Tenant);
        foreach (var module in modules)
        {
            var userModule = new UserModule(module.Name, module.PermissionDetail, module.Tenant, true, user.Id);
            await _repo.CreateAsync(userModule);
        }

        await _repo.SaveChangesAsync();
    }

    public async Task<IResult> RegisterClientUserAsync(RegisterClientRequest request, string origin)
    {

        // var users = await _userManager.Users.ToListAsync();
        var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
        if (userWithSameEmail != null)
        {
            throw new IdentityException(string.Format(_localizer["User Email {0} is already taken."], request.Email));
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            CompanyName = request.CompanyName,
            Address1 = request.Address1,
            Address2 = request.Address2,
            City = request.City,
            State_Region = request.State_Region,
            ZipCode = request.ZipCode,
            Country = request.Country,
            PhoneNumber = request.PhoneNumber,
            Status = request.Status,
            IsActive = true,
            Tenant = RoleConstants.Client,
            ParentID = request.ParentID,
            BrandId = request.BrandId,
            CreatedOn = DateTime.UtcNow

        };
        /*
        if (request.IpAddresses != null && request.IpAddresses.Count > 0)
        {
            foreach (string ipAddress in request.IpAddresses.Distinct())
            {
                var restrictIPId = await _repo.CreateAsync<UserRestrictedIp>(new UserRestrictedIp() { RestrictAccessIPAddress = ipAddress, UserId = user.Id });
                await _repo.SaveChangesAsync();
            }
        }
        */

        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            try
            {
                await _userManager.AddToRoleAsync(user, RoleConstants.Client);
            }
            catch
            {
            }

            var messages = new List<string> { string.Format(_localizer["User {0} Registered."], user.UserName) };
            if (_mailSettings.EnableVerification)
            {
                // send verification email
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
                messages.Add(_localizer[$"Please check {user.Email} to verify your account!"]);
            }

            _jobService.Enqueue(() => BindClientModules(user));

            string message = $"Hello [[fullName]], a new client user is created.";

            // await _notificationService.SendMessageToAdminsHavingPermissionAsync(new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.NEW_USER_REGISTERED, TargetUserTypes = NotificationTargetUserTypes.Admins, Data = new { UserId = user.Id } }, PermissionConstants.Users.Update);
            await _notificationService.SendMessageToSuperAdminsHavingPermissionAsync(new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.NEW_USER_REGISTERED, TargetUserTypes = NotificationTargetUserTypes.SuperAdmins, NotifyModelId = Guid.Parse(user.Id), Data = new { UserId = user.Id } });

            return await Result<string>.SuccessAsync(user.Id, messages: messages);
        }
        else
        {
            throw new IdentityException(_localizer["Validation Errors Occurred."], result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
        }
    }

    public async Task<IResult> RegisterImportClientUserAsync(RegisterClientRequest request, string origin)
    {

        // var users = await _userManager.Users.ToListAsync();
        var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
        if (userWithSameEmail != null)
        {
            return await Result<string>.FailAsync(string.Format(_localizer["User Email {0} is already taken."], request.Email));
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CompanyName = request.CompanyName,
            Address1 = request.Address1,
            Address2 = request.Address2,
            City = request.City,
            State_Region = request.State_Region,
            ZipCode = request.ZipCode,
            Country = request.Country,
            PhoneNumber = request.PhoneNumber,
            Status = request.Status,
            IsActive = true,
            Tenant = "Client",
            ParentID = request.ParentID,
            BrandId = request.BrandId,
            CreatedOn = DateTime.UtcNow

        };
        /*
        if (request.IpAddresses != null && request.IpAddresses.Count > 0)
        {
            foreach (string ipAddress in request.IpAddresses.Distinct())
            {
                var restrictIPId = await _repo.CreateAsync<UserRestrictedIp>(new UserRestrictedIp() { RestrictAccessIPAddress = ipAddress, UserId = user.Id });
                await _repo.SaveChangesAsync();
            }
        }
        */

        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            try
            {
                await _userManager.AddToRoleAsync(user, RoleConstants.Client);
            }
            catch
            {
            }

            var messages = new List<string> { string.Format(_localizer["User {0} Registered."], user.UserName) };
            if (_mailSettings.EnableVerification)
            {
                // send verification email
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
                messages.Add(_localizer[$"Please check {user.Email} to verify your account!"]);
            }

            _jobService.Enqueue(() => BindClientModules(user));

            string message = $"Hello [[fullName]], a new client user is created.";

            // await _notificationService.SendMessageToAdminsHavingPermissionAsync(new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.NEW_USER_REGISTERED, TargetUserTypes = NotificationTargetUserTypes.Admins, Data = new { UserId = user.Id } }, PermissionConstants.Users.Update);
            await _notificationService.SendMessageToSuperAdminsHavingPermissionAsync(new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.NEW_USER_REGISTERED, TargetUserTypes = NotificationTargetUserTypes.SuperAdmins, NotifyModelId = Guid.Parse(user.Id), Data = new { UserId = user.Id } });

            return await Result<string>.SuccessAsync(user.Id, messages: messages);
        }
        else
        {
            return await Result<string>.FailAsync(string.Format(_localizer["Validation Errors Occurred. {0}"], result.Errors.Select(a => _localizer[a.Description].ToString()).ToList()));
        }
    }

    public async Task<IResult> RegisterAsync(RegisterRequest request, string origin)
    {
        var users = await _userManager.Users.ToListAsync();
        var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
        if (userWithSameUserName != null)
        {
            throw new IdentityException(string.Format(_localizer["Username {0} is already taken."], request.UserName));
        }

        var user = new ApplicationUser
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            Tenant = _tenantService.GetCurrentTenant()?.Key
        };
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var userWithSamePhoneNumber = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);
            if (userWithSamePhoneNumber != null)
            {
                throw new IdentityException(string.Format(_localizer["Phone number {0} is already registered."], request.PhoneNumber));
            }
        }

        var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
        if (userWithSameEmail == null)
        {
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                try
                {
                    await _userManager.AddToRoleAsync(user, RoleConstants.Client);
                }
                catch
                {
                }

                var messages = new List<string> { string.Format(_localizer["User {0} Registered."], user.UserName) };
                if (_mailSettings.EnableVerification)
                {
                    // send verification email
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
                    messages.Add(_localizer[$"Please check {user.Email} to verify your account!"]);
                }

                _jobService.Enqueue(() => BindClientModules(user));

                return await Result<string>.SuccessAsync(user.Id, messages: messages);
            }
            else
            {
                throw new IdentityException(_localizer["Validation Errors Occurred."], result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
            }
        }
        else
        {
            throw new IdentityException(string.Format(_localizer["Email {0} is already registered."], request.Email));
        }
    }

    public async Task<IResult<string>> ConfirmEmailAsync(string userId, string code, string tenant)
    {
        var user = await _userManager.Users.IgnoreQueryFilters().Where(a => a.Id == userId && !a.EmailConfirmed && a.Tenant == tenant).FirstOrDefaultAsync();
        if (user == null)
        {
            throw new IdentityException(_localizer["identity.emailverify"]);
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
        {
            return await Result<string>.SuccessAsync(user.Id, string.Format(_localizer["Account Confirmed for E-Mail {0}. You can now use the /api/identity/token endpoint to generate JWT."], user.Email));
        }
        else
        {
            throw new IdentityException(string.Format(_localizer["An error occurred while confirming the user {0}"], user.Email), result.Errors.Select(x => x.Description).ToList());
        }
    }

    public async Task<IResult<string>> ConfirmPhoneNumberAsync(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new IdentityException(_localizer["An error occurred while confirming Mobile Phone."]);
        }

        var result = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, code);
        if (result.Succeeded)
        {
            if (user.EmailConfirmed)
            {
                return await Result<string>.SuccessAsync(user.Id, string.Format(_localizer["Account Confirmed for Phone Number {0}. You can now use the /api/identity/token endpoint to generate JWT."], user.PhoneNumber));
            }
            else
            {
                return await Result<string>.SuccessAsync(user.Id, string.Format(_localizer["Account Confirmed for Phone Number {0}. You should confirm your E-mail before using the /api/identity/token endpoint to generate JWT."], user.PhoneNumber));
            }
        }
        else
        {
            throw new IdentityException(string.Format(_localizer["An error occurred while confirming {0}"], user.PhoneNumber));
        }
    }

    public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
            // Don't reveal that the user does not exist or is not confirmed
            throw new IdentityException(_localizer["An Error has occurred!"]);
        }

        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        string code = await _userManager.GeneratePasswordResetTokenAsync(user);
        const string route = "reset-password";
        origin = await GetDomainApiUriAsync(user, origin);
        var endpointUri = new Uri(string.Concat($"{origin}/", route));
        string passwordResetUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "resetToken", code);
        var userDto = user.Adapt<UserDetailsDto>();

        EmailTemplateDto emailDto = await _templateService.GenerateEmailForgetPassword(userDto, code, passwordResetUrl);

        var mailRequest = new MailRequest
        {
            From = _mailSettings.From,
            Body = emailDto.Body,
            Subject = emailDto.Subject,
            To = new List<string> { request.Email }
        };

        _jobService.Enqueue(() => _mailService.SendAsync(mailRequest));
        return await Result.SuccessAsync(_localizer["Password Reset Mail has been sent to your authorized Email."]);
    }

    public async Task<IResult> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            // Don't reveal that the user does not exist
            throw new IdentityException(_localizer["An Error has occurred!"]);
        }

        // string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
        if (result.Succeeded)
        {
            await _tokenService.RevokeToken(user.Id);
            return await Result.SuccessAsync(_localizer["Password Reset Successful!"]);
        }
        else
        {
            throw new IdentityException(_localizer["An Error has occurred!"]);
        }
    }

    public async Task<IResult> ChangePasswordAsync(ChangePasswordRequest request)
    {
        string userId = _user.GetUserId().ToString();
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return await Result.FailAsync(_localizer["User Not Found."]);
        }

        if (!await _userManager.CheckPasswordAsync(user, request.CurrentPassword))
        {
            return await Result.FailAsync(_localizer["Password is not correct."]);
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.Password);
        if (result.Succeeded)
        {
            await _tokenService.RevokeToken(user.Id);
            return await Result.SuccessAsync(_localizer["Password changes Successful!"]);
        }
        else
        {
            throw new IdentityException(_localizer["An Error has occurred!"]);
        }
    }

    public async Task<IResult> ChangePasswordOtherUserAsync(ChangePasswordOtherUserRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            return await Result.FailAsync(_localizer["User Not Found."]);
        }

        string token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, request.Password);

        if (result.Succeeded)
        {
            await _tokenService.RevokeToken(user.Id);
            return await Result.SuccessAsync(_localizer["Password changes Successful!"]);
        }
        else
        {
            throw new IdentityException(_localizer["An Error has occurred!"]);
        }
    }

    public async Task<IResult> UpdateProfileAsync(UpdateProfileRequest request)
    {
        string userId = _user.GetUserId().ToString();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return await Result.FailAsync(_localizer["User Not Found."]);
        }

        if (request.Image != null)
        {
            user.ImageUrl = _fileStorage.GetBase64Image(request.Image); // UploadAsync<ApplicationUser>(request.Image, FileType.Image);
        }

        user.FullName = request.FullName;
        user.Address1 = request.Address1;
        user.Address2 = request.Address2;
        var identityResult = await _userManager.UpdateAsync(user);
        var errors = identityResult.Errors.Select(e => _localizer[e.Description].ToString()).ToList();
        await _signInManager.RefreshSignInAsync(user);
        return identityResult.Succeeded ? await Result.SuccessAsync() : await Result.FailAsync(errors);

    }

    public async Task<IResult> UpdateProfileAsync(UpdateProfileRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return await Result.FailAsync(_localizer["User Not Found."]);
        }

        if (request.Image != null)
        {
            user.ImageUrl = _fileStorage.GetBase64Image(request.Image);  // .UploadAsync<ApplicationUser>(request.Image, FileType.Image);
        }

        user.FullName = request.FullName;
        user.Status = request.Status;

        if (request.IpAddresses != null && request.IpAddresses.Count > 0)
        {
            var userRestrictedIps = await _repo.FindByConditionAsync<UserRestrictedIp>(m => m.UserId == user.Id);

            foreach (string ipAddress in request.IpAddresses.Distinct())
            {
                if (!userRestrictedIps.Any(m => m.RestrictAccessIPAddress == ipAddress))
                {

                    var restrictIPId = await _repo.CreateAsync<UserRestrictedIp>(new UserRestrictedIp() { RestrictAccessIPAddress = ipAddress, UserId = user.Id });
                    await _repo.SaveChangesAsync();
                }
            }

            await _repo.ClearAsync<UserRestrictedIp>(m => !request.IpAddresses.Contains(m.RestrictAccessIPAddress));
        }
        else
        {
            await _repo.ClearAsync<UserRestrictedIp>(m => m.UserId == user.Id);

        }

        if (request.RecordsToDisplay != user.RecordsToDisplay) user.RecordsToDisplay = request.RecordsToDisplay;
        if (request.ParentID != null && !user.ParentID.NullToString().Equals(request.ParentID)) user.ParentID = request.ParentID;
        if (request.BrandId != null && !user.BrandId.NullToString().Equals(request.BrandId)) user.BrandId = request.BrandId;
        if (request.AdminGroupId != null && !user.AdminGroupId.NullToString().Equals(request.AdminGroupId))
        {
            var group = await _repo.GetByIdAsync<AdminGroup>(Guid.Parse(request.AdminGroupId));

            if (group.IsSuperAdmin)
            {
                // Check if Role Exists
                if (await _roleManager.FindByNameAsync(RoleConstants.SuperAdmin) != null && !await _userManager.IsInRoleAsync(user, RoleConstants.SuperAdmin))
                {
                    await _userManager.AddToRoleAsync(user, RoleConstants.SuperAdmin);
                }
            }
            else
            {
                var superadmingroup = await _repo.GetByIdAsync<AdminGroup>(Guid.Parse(user.AdminGroupId));
                if (superadmingroup.IsSuperAdmin)
                {
                    await _userManager.RemoveFromRoleAsync(user, RoleConstants.SuperAdmin);
                    if (!await _userManager.IsInRoleAsync(user, RoleConstants.Admin))
                    {
                        await _userManager.AddToRoleAsync(user, RoleConstants.Admin);
                    }
                }

            }

            string message = $"Hello [[fullName]], you've been added into admin group {group?.GroupName}";
            await _notificationService.SendMessageToUserAsync(user.Id, new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.ADDED_TO_ADMIN_GROUP, TargetUserTypes = NotificationTargetUserTypes.Admins, NotifyModelId = Guid.Parse(request.AdminGroupId), Data = new { AdminGroupId = request.AdminGroupId } });

            user.AdminGroupId = request.AdminGroupId;
        }

        var identityResult = await _userManager.UpdateAsync(user);
        var errors = identityResult.Errors.Select(e => _localizer[e.Description].ToString()).ToList();
        await _signInManager.RefreshSignInAsync(user);
        return identityResult.Succeeded ? await Result.SuccessAsync() : await Result.FailAsync(errors);

    }

    public async Task<IResult> UpdateEmailAsync(UpdateEmailRequest request)
    {
        string userId = _user.GetUserId().ToString();

        var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);

        if (userWithSameEmail != null && userWithSameEmail.Email == request.Email && userWithSameEmail.Id != userId)
        {
            return await Result.FailAsync(_localizer["Email address is already in used."]);
        }

        if (userWithSameEmail == null || userWithSameEmail.Id == userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return await Result.FailAsync(_localizer["User Not Found."]);
            }

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return await Result.FailAsync(_localizer["Password is not correct."]);
            }

            user.Email = request.Email;
            string phoneNumber = await _userManager.GetEmailAsync(user);
            if (request.Email != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetEmailAsync(user, request.Email);
            }

            var identityResult = await _userManager.UpdateAsync(user);
            var errors = identityResult.Errors.Select(e => _localizer[e.Description].ToString()).ToList();
            await _signInManager.RefreshSignInAsync(user);
            return identityResult.Succeeded ? await Result.SuccessAsync() : await Result.FailAsync(errors);
        }
        else
        {
            return await Result.FailAsync(string.Format(_localizer["Email {0} is already used."], request.Email));
        }
    }

    private async Task<string> GetMobilePhoneVerificationCodeAsync(ApplicationUser user)
    {
        return await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
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
