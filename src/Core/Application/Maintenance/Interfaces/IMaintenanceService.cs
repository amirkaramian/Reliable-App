using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.MaintenanceMode;
using MyReliableSite.Shared.DTOs.Maintenance;

namespace MyReliableSite.Application.Maintenances.Interfaces;

public interface IMaintenanceService : ITransientService
{
    Task<Result<Guid>> CreateMaintenanceAsync(CreateMaintenanceRequest request);
    Task<Result<List<MaintenanceMode>>> GetMaintenanceLogsAsync(string tenant);
    Task<Result<MaintenanceMode>> GetLastMaintenanceLogAsync(string tenant);
}
