using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.Filters;
using MyReliableSite.Shared.DTOs.Notifications;

namespace MyReliableSite.Application.Common.Interfaces;

public interface INotificationService : ITransientService
{
    Task BroadcastExceptMessageAsync(INotificationMessage notification, IEnumerable<string> excludedConnectionIds);

    Task BroadcastMessageAsync(INotificationMessage notification);

    Task SendMessageAsync(INotificationMessage notification);

    Task SendMessageExceptAsync(INotificationMessage notification, IEnumerable<string> excludedConnectionIds);

    Task SendMessageToGroupAsync(INotificationMessage notification, string group);

    Task SendMessageToGroupExceptAsync(INotificationMessage notification, string group, IEnumerable<string> excludedConnectionIds);

    Task SendMessageToGroupsAsync(INotificationMessage notification, IEnumerable<string> groupNames);

    Task SendMessageToUserAsync(string userId, INotificationMessage notification, bool invokeSaveChanges = true);

    Task SendMessageToUsersAsync(IEnumerable<string> userIds, INotificationMessage notification);

    Task<Result<NotificationDto>> GetAsync(Guid id);

    Task<PaginatedResult<NotificationDto>> GetAllAsync(NotificationsListFilter filter);

    Task<PaginatedResult<NotificationDto>> SearchAsync(NotificationsListFilter filter);
    Task<Result<bool>> DeleteAsync(Guid id);
    Task SendMessageToAdminsHavingPermissionAsync(INotificationMessage notification, string permissions);
    Task SendMessageToSuperAdminsHavingPermissionAsync(INotificationMessage notification);

    Task<Result<bool>> SendNotificationBasedOnNotificationTemplateIdAsync(SendBasicNotificationRequest request);
    Task<Result<bool>> SendNotificationByIdAsync(SendNotificationByIdRequest request);

    Task<Result<bool>> ReadNotificationAsync(Guid id);
    Task<Result<bool>> ReadNotificationAsync(List<Guid> ids);
    Task<Result<bool>> UnReadNotificationAsync(Guid id);
}