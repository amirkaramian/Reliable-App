using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Maintenances.Interfaces;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Dashboard;
using MyReliableSite.Shared.DTOs.Maintenance;
using MyReliableSite.Domain.MaintenanceMode;

namespace MyReliableSite.Application.Maintenances.Services;

public class MaintenanceService : IMaintenanceService
{
    private readonly IStringLocalizer<MaintenanceService> _localizer;
    private readonly IRepositoryAsync _repository;

    public MaintenanceService()
    {
    }

    public MaintenanceService(IRepositoryAsync repository, IStringLocalizer<MaintenanceService> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<Result<Guid>> CreateMaintenanceAsync(CreateMaintenanceRequest request)
    {
        var maintenanceMode = new MaintenanceMode(request.ExpirationDateTime, request.Message, request.Status, request.ByPassuserRoles, request.ByPassUsers);
        maintenanceMode.DomainEvents.Add(new StatsChangedEvent());
        var maintenanceModeId = await _repository.CreateAsync<MaintenanceMode>((MaintenanceMode)maintenanceMode);
        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(maintenanceModeId);
    }

    public async Task<Result<List<MaintenanceMode>>> GetMaintenanceLogsAsync(string tenant)
    {
        return await Result<List<MaintenanceMode>>.SuccessAsync(await _repository.GetListAsync<MaintenanceMode>(m => m.Tenant == tenant));

    }

    public async Task<Result<MaintenanceMode>> GetLastMaintenanceLogAsync(string tenant)
    {
        string[] orderby = { "CreatedOn" };
        return await Result<MaintenanceMode>.SuccessAsync(
            await _repository.LastByConditionAsync<MaintenanceMode>(
                m => m.Tenant == tenant, orderby));

    }
}
