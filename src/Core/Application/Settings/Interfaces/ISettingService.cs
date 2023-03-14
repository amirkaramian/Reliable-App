using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Shared.DTOs.Billing;
using MyReliableSite.Shared.DTOs.Products;
using MyReliableSite.Shared.DTOs.Settings;

namespace MyReliableSite.Application.Settings.Interfaces;

public interface ISettingService : ITransientService
{
    Task<Result<Guid>> CreateSettingAsync(CreateSettingRequest request);

    Task<Result<Guid>> UpdateSettingAsync(UpdateSettingRequest request, Guid id);

    Task<Result<SettingDetailsDto>> GetSettingDetailsAsync(Guid id);
    Task<Result<SettingDetailsDto>> GetSettingDetailsAsync(string tenant);

    // Support Settings
    Task<Result<Guid>> CreateSupportSettingAsync(CreateSupportSettingRequest request);

    Task<Result<Guid>> UpdateSupportSettingAsync(UpdateSupportSettingRequest request, Guid id);

    Task<Result<SettingSupportDetailsDto>> GetSupportSettingDetailsAsync(Guid id);
    Task<Result<SettingSupportDetailsDto>> GetSupportSettingDetailsAsync(string tenant);

    // Billing Settings
    Task<Result<Guid>> CreateBillingSettingAsync(CreateBillingSettingRequest request);

    Task<Result<Guid>> UpdateBillingSettingAsync(UpdateBillingSettingRequest request, Guid id);

    Task<Result<SettingBillingDetailsDto>> GetBillingSettingDetailsAsync(Guid id);
    Task<Result<SettingBillingDetailsDto>> GetBillingSettingDetailsAsync(string tenant);

    // User Settings
    Task<Result<string>> CreateUserAppSettingAsync(CreateUserAppSettingRequest request);

    Task<Result<Guid>> UpdateUserAppSettingAsync(UpdateUserAppSettingRequest request, Guid id);
    Task<Result<string>> UpdateAutoBillUserAppSettingAsync(UpdateAutoBillUserAppSettingRequest request, Guid id);

    Task<Result<List<SettingEXL>>> GetSettingListAsync(string userId, DateTime startDate, DateTime endDate);

    Task<Result<SettingUserAppDetailsDto>> GetUserAppSettingDetailsAsync(Guid id);
    Task<Result<SettingUserAppDetailsDto>> GetUserAppSettingByUserIdAsync(string userId);
    Task<Result<List<SettingUserAppDetailsDto>>> GetUserAppSettingDetailsAsync(string tenant);

    Task<Result<Guid>> UpdateCantakeOrdersAsync(Guid id, bool isEnabled);
    Task<Result<Guid>> UpdateAutoAssignOrdersAsync(Guid id, bool isEnabled);
    Task<Result<Guid>> UpdateAvailableForOrderAsync(Guid userid, bool isEnable);
}