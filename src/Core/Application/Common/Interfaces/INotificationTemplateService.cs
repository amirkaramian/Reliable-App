using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.Filters;
using MyReliableSite.Shared.DTOs.Identity;
using MyReliableSite.Shared.DTOs.Notifications;
using MyReliableSite.Shared.DTOs.Notifications.Templates;

namespace MyReliableSite.Application.Common.Interfaces;

public interface INotificationTemplateService : ITransientService
{
    Task<Result<NotificationTemplateDetailsDto>> GetAsync(Guid id);

    Task<PaginatedResult<NotificationTemplateDto>> SearchAsync(NotificatoinTemplatesListFilter filter);

    Task<Result<NotificationTemplateDto>> CreateAsync(CreateNotificationTemplateRequest request);

    Task<Result<NotificationTemplateDetailsDto>> UpdateAsync(Guid id, UpdateNotificationTemplateRequest request);

    Task<Result<bool>> DeleteAsync(Guid id);
}
