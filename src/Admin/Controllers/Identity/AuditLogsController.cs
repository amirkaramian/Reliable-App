using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Auditing;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Auditing;

namespace MyReliableSite.Admin.API.Controllers.Identity;

public class AuditLogsController : BaseController
{
    private readonly IAuditService _auditService;

    public AuditLogsController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    /// <summary>
    /// Retrive the audit log against specific id.
    /// </summary>
    /// <response code="200">Audit Log returns.</response>
    /// <response code="404">Audit Log not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<AuditLogsDetailsDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "AuditLogs", "Read", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.AuditLogs.Read)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var product = await _auditService.GetDetail(id);
        return Ok(product);
    }

    /// <summary>
    /// Retrive my logs.
    /// </summary>
    /// <response code="200">My logs returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("mine")]
    [ProducesResponseType(typeof(PaginatedResult<AuditLogsDto>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "AuditLogs", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
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
    [SwaggerHeader("tenant", "AuditLogs", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetUserLogsAsync(Guid userId, AuditLogsListFilter filter)
    {
        return Ok(await _auditService.GetUserAuditLogsAsync(filter, userId));
    }

    /// <summary>
    /// Search user logs.
    /// </summary>
    /// <response code="200">Audit logs returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<IEnumerable<AuditLogsDetailsDto>>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "AuditLogs", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [HttpPost]
    [MustHavePermission(PermissionConstants.AuditLogs.Search)]
    public async Task<IActionResult> Search(AuditLogsListFilter filter)
    {
        return Ok(await _auditService.SearchAsync(filter));
    }
}
