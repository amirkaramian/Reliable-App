using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Auditing;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Auditing;

namespace MyReliableSite.Client.API.Controllers.Identity;

public class AuditLogsController : BaseController
{
    private readonly ICurrentUser _user;
    private readonly IAuditService _auditService;

    public AuditLogsController(IAuditService auditService, ICurrentUser user)
    {
        _auditService = auditService;
        _user = user;
    }

    /// <summary>
    /// Retrive my logs.
    /// </summary>
    /// <response code="200">My logs returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("mine")]
    [ProducesResponseType(typeof(PaginatedResult<AuditLogsDto>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "AuditLogs", "Search", "Input your tenant to access this API i.e. admin for test", "client", true)]
    public async Task<IActionResult> GetMyLogsAsync(AuditLogsListFilter filter)
    {
        return Ok(await _auditService.GetMyAuditLogsAsync(filter));
    }

    /// <summary>
    /// Retrive specific user logs.
    /// </summary>
    /// <response code="200">User logs returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("user/{userId}")]
    [ProducesResponseType(typeof(PaginatedResult<AuditLogsDto>), 200)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.AuditLogs.Search)]
    [SwaggerHeader("tenant", "AuditLogs", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, false)]
    public async Task<IActionResult> GetUserLogsAsync(Guid userId, AuditLogsListFilter filter)
    {
        return Ok(await _auditService.GetUserAuditLogsAsync(filter, userId));
    }
}
