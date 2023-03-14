using Microsoft.AspNetCore.Mvc;

using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Filters;
using MyReliableSite.Shared.DTOs.Notifications;
using MyReliableSite.Shared.DTOs.Notifications.Templates;

namespace MyReliableSite.Admin.API.Controllers.Notifications;

public class NotificationsTemplateController : BaseController
{
    private readonly INotificationTemplateService _notificationTemplateService;

    public NotificationsTemplateController(INotificationTemplateService notificationTemplateService)
    {
        _notificationTemplateService = notificationTemplateService ?? throw new ArgumentNullException(nameof(notificationTemplateService));
    }

    /// <summary>
    /// Retrive the NotificationTemplate against specific id.
    /// </summary>
    /// <response code="200">NotificationTemplate returns.</response>
    /// <response code="404">NotificationTemplate not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Result<NotificationTemplateDetailsDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.NotificationTemplates.Read)]
    [SwaggerHeader("tenant", "NotificationTemplates", "Read", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetAsync(Guid id) => Ok(await _notificationTemplateService.GetAsync(id));

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">Notifications List returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<NotificationTemplateDto>), 200)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.NotificationTemplates.Search)]
    [SwaggerHeader("tenant", "NotificationTemplates", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> SearchAsync(NotificatoinTemplatesListFilter filter)
    {
        return Ok(await _notificationTemplateService.SearchAsync(filter));
    }

    /// <summary>
    /// Create an NotificationTemplate.
    /// </summary>
    /// <response code="200">NotificationTemplate created.</response>
    /// <response code="400">NotificationTemplate already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<NotificationTemplateDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.NotificationTemplates.Create)]
    [SwaggerHeader("tenant", "NotificationTemplates", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> CreateAsync(CreateNotificationTemplateRequest request)
    {
        return Ok(await _notificationTemplateService.CreateAsync(request));
    }

    /// <summary>
    /// update a specific NotificationTemplate by unique id.
    /// </summary>
    /// <response code="200">NotificationTemplate updated.</response>
    /// <response code="404">NotificationTemplate not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Result<NotificationTemplateDetailsDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.NotificationTemplates.Update)]
    [SwaggerHeader("tenant", "NotificationTemplates", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> UpdateAsync(UpdateNotificationTemplateRequest request, Guid id)
    {
        return Ok(await _notificationTemplateService.UpdateAsync(id, request));
    }

    /// <summary>
    /// Delete a specific NotificationTemplate by unique id.
    /// </summary>
    /// <response code="200">NotificationTemplate deleted.</response>
    /// <response code="404">NotificationTemplate not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(Result<bool>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.NotificationTemplates.Delete)]
    [SwaggerHeader("tenant", "NotificationTemplates", "Delete", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> DeleteAsync(Guid id) => Ok(await _notificationTemplateService.DeleteAsync(id));
}
