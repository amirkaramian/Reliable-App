using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.Auditing;
using MyReliableSite.Shared.DTOs.ManageUserApiKey;
using System.Diagnostics;

namespace MyReliableSite.Application.Auditing;

public interface IAuditService : ITransientService
{
    Task<AuditLogsDetailsDto> GetDetail(Guid id);
    Task<PaginatedResult<AuditLogsDto>> SearchAsync(AuditLogsListFilter filter);
    Task<PaginatedResult<AuditLogsDto>> GetUserAuditLogsAsync(AuditLogsListFilter filter, Guid userId);
    Task<PaginatedResult<AuditLogsDto>> GetMyAuditLogsAsync(AuditLogsListFilter filter);
    Task DeleteOldAuditLogs(int LogRotation);

    Task<Result<List<AuditLogsDto>>> GetAllAsync(string userId, DateTime startDate, DateTime endDate);

}