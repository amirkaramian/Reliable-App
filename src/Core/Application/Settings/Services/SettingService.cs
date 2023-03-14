using Mapster;
using MediatR;
using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Auditing;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Settings.Interfaces;
using MyReliableSite.Application.Specifications;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Products;
using MyReliableSite.Shared.DTOs.Billing;
using MyReliableSite.Shared.DTOs.Products;
using MyReliableSite.Shared.DTOs.Settings;
using static MyReliableSite.Domain.Constants.PermissionConstants;

namespace MyReliableSite.Application.Settings.Services;

public class SettingService : ISettingService
{
    private readonly IRepositoryAsync _repository;
    private readonly IJobService _jobService;
    private readonly IAuditService _auditService;
    private readonly IStringLocalizer<SettingService> _localizer;
    private readonly ICurrentUser _userService;

    public SettingService(IRepositoryAsync repository, IJobService jobService, IAuditService auditService, IStringLocalizer<SettingService> localizer, ICurrentUser userService)
    {
        _repository = repository;
        _jobService = jobService;
        _auditService = auditService;
        _localizer = localizer;
        _userService = userService;
    }

    public async Task<Result<Guid>> CreateSettingAsync(CreateSettingRequest request)
    {
        var setting = new Setting(
            request.DateFormat,
            request.DefaultCountry,
            request.AutoRefreshInterval,
            request.LogRotation,
            request.LogRotationDays,
            request.RequestsIntervalPerIPAfterLimitAdminInSeconds,
            request.EnableAPIAccessAdmin,
            request.EnableAPIAccessClient,
            request.RequestsPerIPAdmin,
            request.RequestsPerIPClient,
            request.LoginRequestsPerIPAdmin,
            request.LoginRequestsPerIPClient,
            request.RequestsIntervalPerIPAfterLimitClientInSeconds,
            request.defaultInactivityMinutesLockAdmin,
            request.defaultInactivityMinutesLockClient,
            request.ForceAdminMFA,
            request.ForceClientMFA,
            request.EnableAdminMFA,
            request.EnableClientMFA,
            request.TrustDeviceinDays,
            request.GoogleAuthenticator,
            request.MicrosoftAuthenticator,
            request.Module1Settings,
            request.Module2Settings,
            request.EnableThirdPartyAPIkeys,
            request.NumberofRequestsPerIpApiKey,
            request.IntervalBeforeNextAPIkeyRequestInSeconds,
            request.LoginIntervalInSeconds_PortalSettings,
            request.EnableLoginIntervalInSeconds_PortalSettings,
            request.BillPrefix,
            request.DefaultBillDueDays,
            request.VAT,
            request.CompanyName,
            request.EnableClientRecaptcha,
            request.EnableAdminRecaptcha,
            request.IsActiveOrPendingProducts,
            request.MaxCreditAmount);

        var settingId = await _repository.CreateAsync(setting);
        await _repository.SaveChangesAsync();

        if (request.LogRotation == true)
        {
            _jobService
                .ScheduleRecurringJobOnDailyCron(
                () => _auditService.DeleteOldAuditLogs(request.LogRotationDays * -1));

        }

        return await Result<Guid>.SuccessAsync(settingId);
    }

    public async Task<Result<Guid>> UpdateSettingAsync(UpdateSettingRequest request, Guid id)
    {
        var setting = await _repository.GetByIdAsync<Setting>(id, null);
        if (setting == null) throw new EntityNotFoundException(string.Format(_localizer["setting.notfound"], id));

        var updatedsetting = setting.Update(
            request.DateFormat,
            request.DefaultCountry,
            request.AutoRefreshInterval,
            request.LogRotation,
            request.LogRotationDays,
            request.RequestsIntervalPerIPAfterLimitAdminInSeconds,
            request.EnableAPIAccessAdmin,
            request.EnableAPIAccessClient,
            request.RequestsPerIPAdmin,
            request.RequestsPerIPClient,
            request.LoginRequestsPerIPAdmin,
            request.LoginRequestsPerIPClient,
            request.RequestsIntervalPerIPAfterLimitClientInSeconds,
            request.defaultInactivityMinutesLockAdmin,
            request.defaultInactivityMinutesLockClient,
            request.ForceAdminMFA,
            request.ForceClientMFA,
            request.EnableAdminMFA,
            request.EnableClientMFA,
            request.TrustDeviceinDays,
            request.GoogleAuthenticator,
            request.MicrosoftAuthenticator,
            request.Module1Settings,
            request.Module2Settings,
            request.EnableThirdPartyAPIkeys,
            request.NumberofRequestsPerIpApiKey,
            request.IntervalBeforeNextAPIkeyRequestInSeconds,
            request.LoginIntervalInSeconds_PortalSettings,
            request.EnableLoginIntervalInSeconds_PortalSettings,
            request.BillPrefix,
            request.DefaultBillDueDays,
            request.VAT,
            request.CompanyName,
            request.EnableClientRecaptcha,
            request.EnableAdminRecaptcha,
            request.IsActiveOrPendingProducts,
            request.MaxCreditAmount);

        await _repository.UpdateAsync(updatedsetting);
        await _repository.SaveChangesAsync();

        if (request.LogRotation == true)
        {
            _jobService
                            .ScheduleRecurringJobOnDailyCron(
                            () => _auditService.DeleteOldAuditLogs(request.LogRotationDays * -1));
        }

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<SettingDetailsDto>> GetSettingDetailsAsync(Guid id)
    {
        var setting = await _repository.GetByIdAsync<Setting, SettingDetailsDto>(id);
        return await Result<SettingDetailsDto>.SuccessAsync(setting);
    }

    public async Task<Result<SettingDetailsDto>> GetSettingDetailsAsync(string tenant)
    {
        var setting = await _repository.QueryFirstOrDefaultAsync<Setting>($"SELECT * FROM dbo.\"Settings\" WHERE \"Tenant\"  = '{tenant}'");
        var mappedSettings = setting.Adapt<SettingDetailsDto>();
        return await Result<SettingDetailsDto>.SuccessAsync(mappedSettings);
    }

    public async Task<Result<Guid>> CreateSupportSettingAsync(CreateSupportSettingRequest request)
    {
        var setting = new SupportSetting(request.MaxNumberOfSubCategories, request.AutoApproveNewArticles);

        var settingId = await _repository.CreateAsync(setting);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(settingId);
    }

    public async Task<Result<Guid>> UpdateSupportSettingAsync(UpdateSupportSettingRequest request, Guid id)
    {
        var setting = await _repository.GetByIdAsync<SupportSetting>(id, null);

        var updatedsetting = setting.Update(request.MaxNumberOfSubCategories, request.AutoApproveNewArticles);

        await _repository.UpdateAsync(updatedsetting);
        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<SettingSupportDetailsDto>> GetSupportSettingDetailsAsync(Guid id)
    {
        var setting = await _repository.GetByIdAsync<SupportSetting, SettingSupportDetailsDto>(id);
        return await Result<SettingSupportDetailsDto>.SuccessAsync(setting);
    }

    public async Task<Result<SettingSupportDetailsDto>> GetSupportSettingDetailsAsync(string tenant)
    {
        var setting = await _repository.FirstByConditionAsync<SupportSetting>(m => m.Tenant.Equals(tenant));
        if (setting == null)
        {
            var settingCreate = new SupportSetting(0, true);
            settingCreate.Tenant = tenant;
            var settingId = await _repository.CreateAsync(settingCreate);
            await _repository.SaveChangesAsync();
            setting = await _repository.FirstByConditionAsync<SupportSetting>(m => m.Tenant.Equals(tenant));
        }

        var mappedSettings = setting.Adapt<SettingSupportDetailsDto>();
        return await Result<SettingSupportDetailsDto>.SuccessAsync(mappedSettings);
    }

    public async Task<Result<Guid>> CreateBillingSettingAsync(CreateBillingSettingRequest request)
    {
        var setting = new BillingSetting(
            request.MaxNumberOfRefunds,
            request.MinOrderAmount,
            request.RefundRetainPercentage,
            request.AutoInvoiceGeneration,
            request.AutoInvoicePriorToDueDateInDays,
            request.EnableProductlevelInvoiceGen,
            request.ProductLevelInvoiceGenPriorToDueDateInDays,
            request.MaxCreditAmount,
            request.IsActiveOrPendingProducts);

        var settingId = await _repository.CreateAsync(setting);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(settingId);
    }

    public async Task<Result<Guid>> UpdateBillingSettingAsync(UpdateBillingSettingRequest request, Guid id)
    {
        var setting = await _repository.GetByIdAsync<BillingSetting>(id, null);

        var updatedsetting = setting.Update(
            request.MaxNumberOfRefunds,
            request.MinOrderAmount,
            request.RefundRetainPercentage,
            request.AutoInvoiceGeneration,
            request.AutoInvoicePriorToDueDateInDays,
            request.EnableProductlevelInvoiceGen,
            request.ProductLevelInvoiceGenPriorToDueDateInDays,
            request.MaxCreditAmount,
            request.IsActiveOrPendingProducts);

        await _repository.UpdateAsync(updatedsetting);
        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<SettingBillingDetailsDto>> GetBillingSettingDetailsAsync(Guid id)
    {
        var setting = await _repository.GetByIdAsync<BillingSetting, SettingBillingDetailsDto>(id);
        return await Result<SettingBillingDetailsDto>.SuccessAsync(setting);
    }

    public async Task<Result<SettingBillingDetailsDto>> GetBillingSettingDetailsAsync(string tenant)
    {
        var setting = await _repository.QueryFirstOrDefaultAsync<BillingSetting>($"SELECT * FROM dbo.\"BillingSettings\" WHERE \"Tenant\"  = '{tenant}'");
        var mappedSettings = setting.Adapt<SettingBillingDetailsDto>();

        return await Result<SettingBillingDetailsDto>.SuccessAsync(mappedSettings);
    }

    #region user app settings
    public async Task<Result<string>> CreateUserAppSettingAsync(CreateUserAppSettingRequest request)
    {
        var user = await _repository.FirstByConditionAsync<UserAppSetting>(m => m.UserId == request.UserId);
        if (user != null)
        {
            return await Result<string>.FailAsync("User Settings already exists");
        }

        var setting = new UserAppSetting(request.ClientStatus, request.RequestPerIPOverride, request.IPRestrictionIntervalOverrideInSeconds, request.APIKeyLimitOverride, request.APIKeyIntervalOverrideInSeconds, request.ExtendSuspensionDate, request.UserId, request.IsActiveOrPendingProduct, request.AuotoBill, request.MaxCreditAmount, request.CanTakeOrders, request.AvallableForOrder, request.AutoAssignOrders);

        Guid settingId = await _repository.CreateAsync(setting);
        await _repository.SaveChangesAsync();

        if (request.RestrictAccessToIPAddress != null && request.RestrictAccessToIPAddress.Count > 0)
        {
            var userRestrictedIps = await _repository.FindByConditionAsync<UserRestrictedIp>(m => m.UserId == request.UserId);

            var restrictAccessToIPAddress = userRestrictedIps?.ToList();

            foreach (string ipAddress in request.RestrictAccessToIPAddress.Distinct())
            {
                if (!restrictAccessToIPAddress.Any(m => m.RestrictAccessIPAddress == ipAddress))
                {

                    var restrictIPId = await _repository.CreateAsync<UserRestrictedIp>(new UserRestrictedIp() { RestrictAccessIPAddress = ipAddress, UserId = request.UserId });
                    await _repository.SaveChangesAsync();
                }
            }

            await _repository.ClearAsync<UserRestrictedIp>(m => !request.RestrictAccessToIPAddress.Contains(m.RestrictAccessIPAddress));
        }
        else
        {
            await _repository.ClearAsync<UserRestrictedIp>(m => m.UserId == request.UserId);

        }

        return await Result<string>.SuccessAsync(settingId.ToString());
    }

    public async Task<Result<Guid>> UpdateUserAppSettingAsync(UpdateUserAppSettingRequest request, Guid id)
    {
        var setting = await _repository.GetByIdAsync<UserAppSetting>(id, null);

        var updatedsetting = setting.Update(request.ClientStatus, request.RequestPerIPOverride, request.IPRestrictionIntervalOverrideInSeconds, request.APIKeyLimitOverride, request.APIKeyIntervalOverrideInSeconds, request.ExtendSuspensionDate, request.UserId, request.IsActiveOrPendingProduct, request.AutoBill, request.MaxCreditAmount, request.CanTakeOrders, request.AvallableForOrder, request.AutoAssignOrders);
        if (request.RestrictAccessToIPAddress != null && request.RestrictAccessToIPAddress.Count > 0)
        {
            var userRestrictedIps = await _repository.FindByConditionAsync<UserRestrictedIp>(m => m.UserId == request.UserId);

            var restrictAccessToIPAddress = userRestrictedIps?.ToList();

            foreach (string ipAddress in request.RestrictAccessToIPAddress.Distinct())
            {
                if (!restrictAccessToIPAddress.Any(m => m.RestrictAccessIPAddress == ipAddress))
                {

                    var restrictIPId = await _repository.CreateAsync<UserRestrictedIp>(new UserRestrictedIp() { RestrictAccessIPAddress = ipAddress, UserId = request.UserId });
                }
            }

            await _repository.ClearAsync<UserRestrictedIp>(m => !request.RestrictAccessToIPAddress.Contains(m.RestrictAccessIPAddress));
        }
        else
        {
            await _repository.ClearAsync<UserRestrictedIp>(m => m.UserId == request.UserId);

        }

        try
        {

            await _repository.UpdateAsync(updatedsetting);
        }
        catch (NothingToUpdateException)
        {
        }
        finally
        {

            await _repository.SaveChangesAsync();
        }

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<SettingUserAppDetailsDto>> GetUserAppSettingDetailsAsync(Guid id)
    {
        var setting = await _repository.GetByIdAsync<UserAppSetting, SettingUserAppDetailsDto>(id);
        if (setting != null)
        {
            var userRestrictedIps = await _repository.FindByConditionAsync<UserRestrictedIp>(m => m.UserId == setting.UserId);

            setting.RestrictAccessToIPAddress = userRestrictedIps?.Select(m => m.RestrictAccessIPAddress).ToList();

        }

        return await Result<SettingUserAppDetailsDto>.SuccessAsync(setting);
    }

    public async Task<Result<SettingUserAppDetailsDto>> GetUserAppSettingByUserIdAsync(string userId)
    {
        var setting = await _repository.FirstByConditionAsync<UserAppSetting>(m => m.UserId == userId);
        var result = setting.Adapt<SettingUserAppDetailsDto>();
        if (result != null)
        {
            var userRestrictedIps = await _repository.FindByConditionAsync<UserRestrictedIp>(m => m.UserId == setting.UserId);

            result.RestrictAccessToIPAddress = userRestrictedIps?.Select(m => m.RestrictAccessIPAddress).ToList();

        }

        return await Result<SettingUserAppDetailsDto>.SuccessAsync(result);
    }

    public async Task<Result<List<SettingUserAppDetailsDto>>> GetUserAppSettingDetailsAsync(string tenant)
    {
        var setting = await _repository.QueryAsync<UserAppSetting>($"SELECT * FROM dbo.\"UserAppSettings\" WHERE \"Tenant\"  = '{tenant}'");
        var mappedSettings = setting.Adapt<List<SettingUserAppDetailsDto>>();
        if (mappedSettings != null)
        {
            foreach (var item in mappedSettings)
            {
                var userRestrictedIps = await _repository.FindByConditionAsync<UserRestrictedIp>(m => m.UserId == item.UserId);

                item.RestrictAccessToIPAddress = userRestrictedIps?.Select(m => m.RestrictAccessIPAddress).ToList();
            }
        }

        return await Result<List<SettingUserAppDetailsDto>>.SuccessAsync(mappedSettings);
    }

    public async Task<Result<string>> UpdateAutoBillUserAppSettingAsync(UpdateAutoBillUserAppSettingRequest request, Guid id)
    {
        var settings = await _repository.FindByConditionAsync<UserAppSetting>(x => x.Id == id && x.UserId == _userService.GetUserId().ToString());
        if (settings != null && settings.Any())
        {
            var setting = settings.First();
            setting.AutoBill = request.AutoBill;
            await _repository.UpdateAsync(setting);
            await _repository.SaveChangesAsync();
            return await Result<string>.SuccessAsync(id.ToString());
        }

        return await Result<string>.FailAsync("User Settings already exists");
    }

    public async Task<Result<List<SettingEXL>>> GetSettingListAsync(string userId, DateTime startDate, DateTime endDate)
    {

        var settings = await _repository.QueryWithDtoAsync<SettingEXL>($@"SELECT B.*
                                                                                                        FROM Bills B
                                                                                                        WHERE ((CONVERT(date, [B].[CreatedOn]) >= '{startDate}') AND (CONVERT(date, [B].[CreatedOn]) <= '{endDate}')) and DeletedOn is null and UserId = '{userId}' ORDER BY B.CreatedOn ASC");
        return await Result<List<SettingEXL>>.SuccessAsync(settings.ToList());
    }

    public async Task<Result<Guid>> UpdateCantakeOrdersAsync(Guid userid, bool isEnabled)
    {
        var setting = await _repository.FindByConditionAsync<UserAppSetting>(x => x.UserId == userid.ToString());
        Guid id = Guid.Empty;
        UserAppSetting updatedsetting = null;
        if (setting == null || !setting.Any())
        {
            updatedsetting = new UserAppSetting();
            updatedsetting.UserId = userid.ToString();
            updatedsetting.CanTakeOrders = isEnabled;
            id = await _repository.CreateAsync(updatedsetting);
        }
        else
        {
            updatedsetting = setting.First();
            updatedsetting.CanTakeOrders = isEnabled;
            id = updatedsetting.Id;
            await _repository.UpdateAsync<UserAppSetting>(updatedsetting);
        }

        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> UpdateAutoAssignOrdersAsync(Guid userid, bool isEnable)
    {
        var setting = await _repository.FindByConditionAsync<UserAppSetting>(x => x.UserId == userid.ToString());
        Guid id = Guid.Empty;
        UserAppSetting updatedsetting = null;
        if (setting == null || !setting.Any())
        {
            updatedsetting = new UserAppSetting();
            updatedsetting.UserId = userid.ToString();
            updatedsetting.AutoAssignOrders = isEnable;
            id = await _repository.CreateAsync(updatedsetting);
        }
        else
        {
            updatedsetting = setting.First();
            updatedsetting.AutoAssignOrders = isEnable;
            id = updatedsetting.Id;
            await _repository.UpdateAsync(updatedsetting);
        }

        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> UpdateAvailableForOrderAsync(Guid userid, bool isEnable)
    {
        var setting = await _repository.FindByConditionAsync<UserAppSetting>(x => x.UserId == userid.ToString());
        Guid id = Guid.Empty;
        UserAppSetting updatedsetting = null;
        if (setting == null || !setting.Any())
        {
            updatedsetting = new UserAppSetting();
            updatedsetting.UserId = userid.ToString();
            updatedsetting.AvaillableForOrders = isEnable;
            id = await _repository.CreateAsync(updatedsetting);
        }
        else
        {
            updatedsetting = setting.First();
            updatedsetting.AvaillableForOrders = isEnable;
            id = updatedsetting.Id;
            await _repository.UpdateAsync(updatedsetting);
        }

        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(id);
    }
}
#endregion
