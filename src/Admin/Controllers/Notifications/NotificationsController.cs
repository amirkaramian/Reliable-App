using Microsoft.AspNetCore.Mvc;

using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Filters;
using MyReliableSite.Shared.DTOs.Notifications;

namespace MyReliableSite.Admin.API.Controllers.Notifications;

public class NotificationsController : BaseController
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }

    /// <summary>
    /// Retrive the Notification against specific id.
    /// </summary>
    /// <response code="200">Notification returns.</response>
    /// <response code="404">Notification not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<NotificationDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpGet("{id:guid}")]
    [MustHavePermission(PermissionConstants.Notifications.Read)]
    [SwaggerHeader("tenant", "Notifications", "Read", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetAsync(Guid id) => Ok(await _notificationService.GetAsync(id));

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">Notifications List returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<NotificationDto>), 200)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Notifications.Search)]
    [SwaggerHeader("tenant", "Notifications", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> SearchAsync(NotificationsListFilter filter)
    {
        return Ok(await _notificationService.SearchAsync(filter));
    }

    /// <summary>
    /// Send a notification to list of Admins/Client with fields like [[fullName]] etc based on notification template id.
    /// </summary>
    /// <response code="200">Notifications List returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("send")]
    [ProducesResponseType(typeof(Result<bool>), 200)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Notifications.Create)]
    [SwaggerHeader("tenant", "Notifications", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> SendNotificationBasedOnNotificationTemplateId(SendBasicNotificationRequest request)
    {
        return Ok(await _notificationService.SendNotificationBasedOnNotificationTemplateIdAsync(request));
    }

    /// <summary>
    /// Delete a specific Notification by unique id.
    /// </summary>
    /// <response code="200">Notification deleted.</response>
    /// <response code="404">Notification not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<bool>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpDelete("{id:guid}")]
    [MustHavePermission(PermissionConstants.Notifications.Delete)]
    [SwaggerHeader("tenant", "Notifications", "Delete", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> DeleteAsync(Guid id) => Ok(await _notificationService.DeleteAsync(id));

    /// <summary>
    /// Set Read a specific Notification by unique id.
    /// </summary>
    /// <response code="200">Notification read succesfully.</response>
    /// <response code="404">Notification not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<bool>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpPut("{id:guid}/read")]
    [MustHavePermission(PermissionConstants.Notifications.Update)]
    [SwaggerHeader("tenant", "Notifications", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> ReadNotification(Guid id) => Ok(await _notificationService.ReadNotificationAsync(id));

    /// <summary>
    /// Set Read Notification in bulk.
    /// </summary>
    /// <response code="200">Notification read succesfully.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<bool>), 200)]
    [ProducesResponseType(500)]
    [HttpPut("read")]
    [MustHavePermission(PermissionConstants.Notifications.Update)]
    [SwaggerHeader("tenant", "Notifications", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> ReadNotification(List<Guid> ids) => Ok(await _notificationService.ReadNotificationAsync(ids));
}
