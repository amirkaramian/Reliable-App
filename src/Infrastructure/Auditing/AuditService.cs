using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Auditing;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Billing.Events;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.ManageUserApiKey;
using MyReliableSite.Domain.Tickets;
using MyReliableSite.Infrastructure.Identity.Models;
using MyReliableSite.Infrastructure.Persistence.Contexts;
using MyReliableSite.Shared.DTOs.Auditing;
using MyReliableSite.Shared.DTOs.ManageUserApiKey;
using MyReliableSite.Shared.DTOs.Tickets;
using MyReliableSite.Shared.DTOs.Transaction;

namespace MyReliableSite.Infrastructure.Auditing;

public class AuditService : IAuditService
{
    private readonly IRepositoryAsync _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer<AuditService> _localizer;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEventService _eventService;
    private readonly ILogger<AuditService> _logger;
    public AuditService(IRepositoryAsync repository, ICurrentUser currentUser, IStringLocalizer<AuditService> localizer, UserManager<ApplicationUser> userManager, IEventService eventService, ILogger<AuditService> logger)
    {
        _currentUser = currentUser;
        _repository = repository;
        _localizer = localizer;
        _userManager = userManager;
        _eventService = eventService;
        _logger = logger;
    }

    public async Task<AuditLogsDetailsDto> GetDetail(Guid id)
    {
        var log = await _repository.GetByIdAsync<Trail>(id);
        if (log == null) throw new EntityNotFoundException(string.Format(_localizer["auditlogs.notfound"], id));

        return log.Adapt<AuditLogsDetailsDto>();
    }

    public async Task<PaginatedResult<AuditLogsDto>> GetMyAuditLogsAsync(AuditLogsListFilter filter)
    {
        Guid userId = _currentUser.GetUserId();

        return await GetUserAuditLogsAsync(filter, userId);
    }

    public async Task<PaginatedResult<AuditLogsDto>> SearchAsync(AuditLogsListFilter filter)
    {
        var filters = new Filters<Trail>();

        var result = await _repository.GetSearchResultsAsync<Trail, AuditLogsDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword, x => x.Type == "Create" || x.Type == "Update" || x.Type == "Delete");

        result.Data = ParseAuditLog(result.Data);

        return result;
    }

    public async Task<PaginatedResult<AuditLogsDto>> GetUserAuditLogsAsync(AuditLogsListFilter filter, Guid userId)
    {
        var filters = new Filters<Trail>();

        var result = await _repository.GetSearchResultsAsync<Trail, AuditLogsDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword, x => x.UserId == userId && (x.Type == "Create" || x.Type == "Update" || x.Type == "Delete"));

        result.Data = ParseAuditLog(result.Data);

        return result;
    }

    private List<AuditLogsDto> ParseAuditLog(List<AuditLogsDto> auditLogs)
    {
        if (auditLogs != null && auditLogs.Count() > 0)
        {
            var userIds = auditLogs.Select(x => x.UserId.ToString().ToLower()).ToList();

            var userDetails = _userManager.Users.Where(usr => userIds.Contains(usr.Id.ToLower())).ToList();

            foreach (var log in auditLogs)
            {
                if (userDetails != null)
                {
                    var user = userDetails.FirstOrDefault(x => x.Id.ToLower() == log.UserId.ToString().ToLower());

                    if (user != null)
                    {
                        log.FullName = user.FullName;
                        log.Text = $"{log.FullName} has {log.Type}d the {log.TableName}.";
                    }
                    else
                    {
                        log.Text = $"The {log.TableName} has been {log.Type}d.";
                    }
                }
                else
                {
                    log.Text = $"The {log.TableName} has been {log.Type}d.";
                }

                log.CreatedDaysAgo = (DateTime.UtcNow - log.DateTime).TotalDays;
            }
        }

        return auditLogs;
    }

    public async Task DeleteOldAuditLogs(int LogRotation)
    {
        var logs = await _repository.FindByConditionAsync<Trail>(m => m.DateTime <= DateTime.UtcNow.AddDays(LogRotation));
        foreach (var item in logs)
        {
            await _repository.RemoveAsync(item);
        }

        int rows = await _repository.SaveChangesAsync();
        await _eventService.PublishAsync(new StatsChangedEvent());
        _logger.LogInformation("Rows affected: {rows} ", rows.ToString());
    }

    public async Task<Result<List<AuditLogsDto>>> GetAllAsync(string userId, DateTime startDate, DateTime endDate)
    {

        var auditlog = await _repository.QueryWithDtoAsync<AuditLogsDto>($@"SELECT AT.*
                                                                                                        FROM AuditTrails AT
                                                                                                        WHERE ((CONVERT(date, [AT].[DateTime]) >= '{startDate}') AND (CONVERT(date, [AT].[DateTime]) <= '{endDate}')) and UserId = '{userId}'
                                                                                                        ORDER BY AT.DateTime ASC");
        return await Result<List<AuditLogsDto>>.SuccessAsync(auditlog.ToList());
    }
}