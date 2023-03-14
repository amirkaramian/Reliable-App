using Mapster;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Identity.Interfaces;
using MyReliableSite.Application.Multitenancy;
using MyReliableSite.Application.Storage;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.ArticleFeedbacks;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Billing.Events.Notifications;
using MyReliableSite.Domain.Categories;
using MyReliableSite.Domain.Dashboard;
using MyReliableSite.Domain.Products;
using MyReliableSite.Domain.Tickets;
using MyReliableSite.Infrastructure.Hubs;
using MyReliableSite.Shared.DTOs.ArticleFeedbacks;
using MyReliableSite.Shared.DTOs.Categories;
using MyReliableSite.Shared.DTOs.Notifications;
using MyReliableSite.Shared.DTOs.Notifications.Enums;
using MyReliableSite.Shared.DTOs.Orders;
using MyReliableSite.Shared.DTOs.Products;
using MyReliableSite.Shared.DTOs.Tickets;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static MyReliableSite.Domain.Constants.PermissionConstants;

namespace MyReliableSite.Infrastructure.Common.Services;

public class NotificationService : INotificationService
{
    private readonly IFileStorageService _file;
    private readonly IHubContext<NotificationHub> _notificationHubContext;
    private readonly ITenantService _tenantService;
    private readonly IRepositoryAsync _repository;
    private readonly IStringLocalizer<NotificationService> _localizer;
    private readonly IRoleClaimsService _roleClaimsService;
    private readonly IUserService _userService;
    private readonly IAdminGroupService _adminGroupService;
    private readonly ICurrentUser _currentUser;
    public NotificationService(IHubContext<NotificationHub> notificationHubContext, ITenantService tenantService, ICurrentUser currentUser, IRepositoryAsync repository, IStringLocalizer<NotificationService> localizer, IRoleClaimsService roleClaimsService, IUserService userService, IFileStorageService file, IAdminGroupService adminGroupService)
    {
        _notificationHubContext = notificationHubContext ?? throw new ArgumentNullException(nameof(notificationHubContext));
        _tenantService = tenantService ?? throw new ArgumentNullException(nameof(tenantService));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        _roleClaimsService = roleClaimsService ?? throw new ArgumentNullException(nameof(roleClaimsService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _file = file ?? throw new ArgumentNullException(nameof(file));
        _adminGroupService = adminGroupService ?? throw new ArgumentNullException(nameof(adminGroupService));
        _currentUser = currentUser;
    }

    #region RootTenantMethods

    public async Task BroadcastMessageAsync(INotificationMessage notification)
    {
        await _notificationHubContext.Clients.All.SendAsync(notification.MessageType, notification);
    }

    public async Task BroadcastExceptMessageAsync(INotificationMessage notification, IEnumerable<string> excludedConnectionIds)
    {
        await _notificationHubContext.Clients.AllExcept(excludedConnectionIds).SendAsync(notification.MessageType, notification);
    }

    #endregion RootTenantMethods

    public async Task SendMessageAsync(INotificationMessage notification)
    {
        var tenant = _tenantService.GetCurrentTenant();
        await _notificationHubContext.Clients.Group($"GroupTenant-{tenant.Key}").SendAsync(notification.MessageType, notification);
    }

    public async Task SendMessageExceptAsync(INotificationMessage notification, IEnumerable<string> excludedConnectionIds)
    {
        var tenant = _tenantService.GetCurrentTenant();
        await _notificationHubContext.Clients.GroupExcept($"GroupTenant-{tenant.Key}", excludedConnectionIds).SendAsync(notification.MessageType, notification);
    }

    public async Task SendMessageToGroupAsync(INotificationMessage notification, string group)
    {
        await _notificationHubContext.Clients.Group(group).SendAsync(notification.MessageType, notification);
    }

    public async Task SendMessageToGroupsAsync(INotificationMessage notification, IEnumerable<string> groupNames)
    {
        await _notificationHubContext.Clients.Groups(groupNames).SendAsync(notification.MessageType, notification);
    }

    public async Task SendMessageToGroupExceptAsync(INotificationMessage notification, string group, IEnumerable<string> excludedConnectionIds)
    {
        await _notificationHubContext.Clients.GroupExcept(group, excludedConnectionIds).SendAsync(notification.MessageType, notification);
    }

    public async Task SendMessageToUserAsync(string userId, INotificationMessage notification, bool invokeSaveChanges = true)
    {
        DateTime sentAt = DateTime.UtcNow;
        await _notificationHubContext.Clients.User(userId).SendAsync(notification.MessageType, notification);

        var notificationToAdd = new Notification(Guid.Parse(userId), notification.NotificationType, NotificationStatus.Sent, notification.TargetUserTypes, sentAt, notification.Message, notification.Title, null, notification.NotifyModelId);
        await _repository.CreateAsync<Notification>(notificationToAdd);

        if (invokeSaveChanges)
            await _repository.SaveChangesAsync();
    }

    public async Task SendMessageToUsersAsync(IEnumerable<string> userIds, INotificationMessage notification)
    {
        DateTime sentAt = DateTime.UtcNow;
        await _notificationHubContext.Clients.Users(userIds).SendAsync(notification.MessageType, notification);

        // Store notification into Notification table for future
        foreach (string userId in userIds)
        {
            var notificationToAdd = new Notification(Guid.Parse(userId), notification.NotificationType, NotificationStatus.Sent, notification.TargetUserTypes, sentAt, notification.Message, notification.Title, null, notification.NotifyModelId);
            await _repository.CreateAsync<Notification>(notificationToAdd);
        }

        if (userIds.Any())
        {
            await _repository.SaveChangesAsync();
        }
    }

    public async Task SendMessageToUsersAsync(IEnumerable<Guid> userIds, INotificationMessage notification)
    {
        DateTime sentAt = DateTime.UtcNow;
        await _notificationHubContext.Clients.Users(userIds.Select(g => g.ToString()).ToList()).SendAsync(notification.MessageType, notification);

        // Store notification into Notification table for future
        foreach (Guid userId in userIds)
        {
            var notificationToAdd = new Notification(userId, notification.NotificationType, NotificationStatus.Sent, notification.TargetUserTypes, sentAt, notification.Message, notification.Title, null, notification.NotifyModelId);
            await _repository.CreateAsync<Notification>(notificationToAdd);
        }

        if (userIds.Any())
        {
            await _repository.SaveChangesAsync();
        }
    }

    public async Task SendMessageToAdminsHavingPermissionAsync(INotificationMessage notification, string permissions)
    {
        var users = await _roleClaimsService.GetAllUsersHasPermissions(permissions);
        await SendMessageToUsersAsync(users, notification);
    }

    public async Task SendMessageToSuperAdminsHavingPermissionAsync(INotificationMessage notification)
    {
        var admingroup = await _adminGroupService.GetSuperAdminGroupAsync();
        if (admingroup != null && admingroup.Data != null)
        {
            var allUsers = await _userService.GetAllUsersofAdminGroupAsync(admingroup.Data.Id.ToString());

            await SendMessageToUsersAsync(allUsers.Data.Select(m => m.Id), notification);

        }
    }

    public async Task<Result<NotificationDto>> GetAsync(Guid id)
    {
        var toReturn = await _repository.GetByIdAsync<Notification>(id);

        if (toReturn == null) throw new EntityNotFoundException(string.Format(_localizer["notification.notfound"], id));

        var notification = toReturn.Adapt<NotificationDto>();

        if (notification != null)
        {
            var usersData = await _userService.GetUserProfileAsync(Convert.ToString(notification.ToUserId));

            if (usersData != null && usersData.Data != null)
            {
                notification.FullName = usersData.Data.FullName;

                if (!string.IsNullOrEmpty(usersData.Data.ImageUrl))
                {
                    notification.UserImage = await _file.ReturnBase64StringOfImageFileAsync(usersData.Data.ImageUrl);
                }
            }
        }

        return await Result<NotificationDto>.SuccessAsync(notification);
    }

    public async Task<PaginatedResult<NotificationDto>> GetAllAsync(NotificationsListFilter filter)
    {
        PaginatedResult<NotificationDto> notificationDtos = null;

        if (filter.AdvancedSearch != null && filter.AdvancedSearch.Fields.FirstOrDefault(x => x == "status") != null)
        {
            string status = filter.AdvancedSearch?.Keyword;
            var statusEnum = (NotificationStatus)Convert.ToInt32(status);

            notificationDtos = await _repository.GetSearchResultsAsync<Notification, NotificationDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, null, filter.Keyword, x => x.ToUserId == _currentUser.GetUserId() && x.Status == statusEnum && ((filter.StartDate == null || x.CreatedOn >= filter.StartDate) && (filter.EndDate == null || x.CreatedOn <= filter.EndDate)));
        }
        else
        {
            notificationDtos = await _repository.GetSearchResultsAsync<Notification, NotificationDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword, x => x.ToUserId == _currentUser.GetUserId() && (filter.StartDate == null || x.CreatedOn >= filter.StartDate) && (filter.EndDate == null || x.CreatedOn <= filter.EndDate));
        }

        notificationDtos.Data.ForEach(async item =>
        {
            if (item.ToUserId.HasValue)
            {
                var usersData = _userService.GetUserProfileAsync(Convert.ToString(item.ToUserId)).Result;

                if (usersData != null && usersData.Data != null)
                {
                    item.FullName = usersData.Data.FullName;
                    item.Body = item.Body.Replace("[[fullName]]", item.FullName);
                    if (!string.IsNullOrEmpty(usersData.Data.ImageUrl))
                    {
                        item.UserImage = await _file.ReturnBase64StringOfImageFileAsync(usersData.Data.ImageUrl);
                    }
                }
            }

            if (item.NotifyModelId.HasValue)
            {
                switch (item.Type)
                {
                    case NotificationType.ORDER:
                    case NotificationType.ORDER_CREATED:
                    case NotificationType.ORDER_UPDATED:
                        var order = _repository.GetByIdAsync<Order>(item.NotifyModelId.Value).Result;
                        if (order != null)
                        {
                            if (item.Orders == null)
                                item.Orders = new List<OrderDto>();
                            var orderDto = order.Adapt<OrderDto>();
                            item.Orders.Add(orderDto);
                        }

                        break;
                    case NotificationType.TICKET:
                    case NotificationType.TICKET_CREATED:
                    case NotificationType.TICKET_UPDATED:
                        var ticket = _repository.GetByIdAsync<Ticket>(item.NotifyModelId.Value).Result;
                        if (ticket != null)
                        {
                            if (item.Tickets == null)
                                item.Tickets = new List<TicketDto>();
                            item.Tickets.Add(ticket.Adapt<TicketDto>());
                        }

                        break;
                    case NotificationType.BILLS:
                    case NotificationType.BILL_CREATED:
                        var bill = _repository.GetByIdAsync<Bill>(item.NotifyModelId.Value).Result;
                        if (bill != null)
                        {
                            if (item.Bills == null)
                                item.Bills = new List<BillDto>();
                            item.Bills.Add(bill.Adapt<BillDto>());
                        }

                        break;
                    case NotificationType.PRODUCT_STATUS_UPDATED:
                        var product = _repository.GetByIdAsync<Product>(item.NotifyModelId.Value).Result;
                        if (product != null)
                        {
                            if (item.Products == null)
                                item.Products = new List<ProductDto>();
                            item.Products.Add(product.Adapt<ProductDto>());
                        }

                        break;
                    case NotificationType.ARTICLE_FEEDBACK_ADDED:
                    case NotificationType.ARTICLE_FEEDBACK_COMMENT_ADDED:
                    case NotificationType.ARTICLE_FEEDBACK_COMMENT_REPLY_ADDED:
                        var feedback = _repository.GetByIdAsync<ArticleFeedback>(item.NotifyModelId.Value).Result;
                        if (feedback != null)
                        {
                            if (item.ArticleFeedbacks == null)
                                item.ArticleFeedbacks = new List<ArticleFeedbackDto>();
                            item.ArticleFeedbacks.Add(feedback.Adapt<ArticleFeedbackDto>());
                        }

                        break;
                    case NotificationType.CATEGORY:
                    case NotificationType.CATEGORY_GENERATOR:
                        var category = _repository.GetByIdAsync<Category>(item.NotifyModelId.Value).Result;
                        if (category != null)
                        {
                            if (item.Categories != null)
                                item.Categories = new List<CategoryDto>();
                            item.Categories.Add(category.Adapt<CategoryDto>());
                        }

                        break;
                    default:
                        break;
                }
            }

        });
        if (filter.OrderBy == null || !filter.OrderBy.Any())
        {
            if (notificationDtos.Data != null || notificationDtos.Data.Any())
                notificationDtos.Data = notificationDtos.Data.OrderByDescending(x => x.SentAt).ToList();
        }

        return notificationDtos;
    }

    public async Task<PaginatedResult<NotificationDto>> SearchAsync(NotificationsListFilter filter)
    {
        PaginatedResult<NotificationDto> notifications = null;

        if (filter.AdvancedSearch != null && filter.AdvancedSearch.Fields.FirstOrDefault(x => x == "status") != null)
        {
            string status = filter.AdvancedSearch?.Keyword;
            var statusEnum = (NotificationStatus)Convert.ToInt32(status);

            notifications = await _repository.GetSearchResultsAsync<Notification, NotificationDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, null, filter.Keyword, x => x.Status == statusEnum && ((filter.StartDate == null || x.CreatedOn >= filter.StartDate) && (filter.EndDate == null || x.CreatedOn <= filter.EndDate)));
        }
        else
        {
            notifications = await _repository.GetSearchResultsAsync<Notification, NotificationDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword, x => (filter.StartDate == null || x.CreatedOn >= filter.StartDate) && (filter.EndDate == null || x.CreatedOn <= filter.EndDate));
        }

        if (notifications != null && notifications.Data != null && notifications.Data.Any())
        {
            var allUsers = await _userService.GetAllAsync(notifications.Data.Select(x => x.ToUserId?.ToString()));

            if (allUsers != null && allUsers.Data != null)
            {
                var users = allUsers.Data;

                foreach (var item in notifications.Data)
                {
                    item.FullName = users.FirstOrDefault(x => x.Id == item.ToUserId).FullName;

                    if (!string.IsNullOrEmpty(users.FirstOrDefault(x => x.Id == item.ToUserId).ImageUrl))
                    {
                        item.UserImage = await _file.ReturnBase64StringOfImageFileAsync(users.FirstOrDefault(x => x.Id == item.ToUserId).ImageUrl);
                    }
                }
            }
        }

        return notifications;
    }

    public async Task<Result<bool>> SendNotificationBasedOnNotificationTemplateIdAsync(SendBasicNotificationRequest request)
    {
        var notificationTemplate = await _repository.GetByIdAsync<NotificationTemplate>(request.NotificationTemplateId);

        string body = notificationTemplate.Body;
        BasicNotification basicNotification = new BasicNotification()
        {
            Label = BasicNotification.LabelType.Information,
            Message = body,
            NotificationType = request.NotificationType,
            TargetUserTypes = request.TargetUserTypes
        };

        DateTime sentAt = DateTime.UtcNow;
        await _notificationHubContext.Clients.Users(request.ToUserIds).SendAsync(basicNotification.MessageType, basicNotification);

        // Parse messages using common fields or params like [[firstName]], [[userName]] etc
        var parsedMessages = await ParseMessageUsingFields(basicNotification.Message, request.ToUserIds, request.TargetUserTypes);

        // Store notification into Notification table for future
        foreach (var item in parsedMessages)
        {
            var notificationToAdd = new Notification(Guid.Parse(item.UserId), basicNotification.NotificationType, NotificationStatus.Sent, basicNotification.TargetUserTypes, sentAt, item.Body, basicNotification.Title, null, basicNotification.NotifyModelId);
            await _repository.CreateAsync<Notification>(notificationToAdd);
        }

        if (parsedMessages.Any())
        {
            await _repository.SaveChangesAsync();
        }

        return await Result<bool>.SuccessAsync(true);
    }

    public async Task<Result<bool>> SendNotificationByIdAsync(SendNotificationByIdRequest request)
    {
        var notificationToSend = await _repository.GetByIdAsync<Notification>(request.NotificationId);
        if (notificationToSend == null) throw new EntityNotFoundException(string.Format(_localizer["notification.notfound"], request.NotificationId));

        string body = notificationToSend.Body;
        BasicNotification basicNotification = new BasicNotification()
        {
            Label = BasicNotification.LabelType.Information,
            Message = body,
            NotificationType = notificationToSend.Type,
            TargetUserTypes = notificationToSend.TargetUserTypes
        };

        await _notificationHubContext.Clients.Users(request.ToUserIds).SendAsync(basicNotification.MessageType, basicNotification);

        notificationToSend = notificationToSend.UpdateSent(DateTime.UtcNow);
        await _repository.UpdateAsync(notificationToSend);
        notificationToSend.DomainEvents.Add(new NotificationUpdatedEvent(notificationToSend));

        int result = await _repository.SaveChangesAsync();
        if (result > 0)
            return await Result<bool>.SuccessAsync(true);
        else
            return await Result<bool>.SuccessAsync(false);

    }

    private async Task<List<(string UserId, string Body)>> ParseMessageUsingFields(string body, IEnumerable<string> userIds, NotificationTargetUserTypes targetUserTypes)
    {
        List<(string UserId, string Body)> list = new List<(string UserId, string Body)>();

        if (userIds != null)
        {
            var usersDetails = await _userService.GetAllAsync(userIds);
            var adminRoleUsers = await _userService.GetAllByUserRoleAsync("Admin");

            List<string> adminRoleTenants = new List<string>();
            List<Setting> adminSettings = new List<Setting>();

            if (adminRoleUsers != null && adminRoleUsers.Data != null)
            {
                adminRoleTenants = adminRoleUsers.Data.Select(x => x.Tenant.ToLower()).Distinct().ToList();

                var settins = await _repository.FindByConditionAsync<Setting>(x => adminRoleTenants.Contains(x.Tenant.ToLower()));
                adminSettings = settins.ToList();
            }

            if (usersDetails != null && usersDetails.Data != null && usersDetails.Data.Any())
            {
                foreach (var user in usersDetails.Data)
                {
                    string parsedBody = body;

                    parsedBody = parsedBody.Replace("[[firstName]]", user.FirstName);
                    parsedBody = parsedBody.Replace("[[lastName]]", user.LastName);
                    parsedBody = parsedBody.Replace("[[fullName]]", user.FullName);
                    parsedBody = parsedBody.Replace("[[email]]", user.Email);
                    parsedBody = parsedBody.Replace("[[phoneNumber]]", user.PhoneNumber);
                    parsedBody = parsedBody.Replace("[[userName]]", user.UserName);
                    parsedBody = parsedBody.Replace("[[address1]]", user.Address1);
                    parsedBody = parsedBody.Replace("[[address2]]", user.Address2);

                    if (targetUserTypes == NotificationTargetUserTypes.Clients)
                    {
                        parsedBody = parsedBody.Replace("[[companyName]]", user.CompanyName);
                    }
                    else if (targetUserTypes == NotificationTargetUserTypes.Admins)
                    {
                        parsedBody = parsedBody.Replace("[[companyName]]", adminSettings.FirstOrDefault(x => x.Tenant.ToLower() == user.Tenant.ToLower())?.CompanyName);
                    }
                }
            }
        }

        return list;
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var toDelete = await _repository.RemoveByIdAsync<Notification>(id);
        if (toDelete == null) throw new EntityNotFoundException(string.Format(_localizer["notification.notfound"], id));

        toDelete.DomainEvents.Add(new NotificationDeletedEvent(toDelete));
        toDelete.DomainEvents.Add(new StatsChangedEvent());

        await _repository.SaveChangesAsync();
        return await Result<bool>.SuccessAsync(await _repository.SaveChangesAsync() > 0);
    }

    public async Task<Result<bool>> ReadNotificationAsync(Guid id)
    {
        var toUpdate = await _repository.GetByIdAsync<Notification>(id);
        if (toUpdate == null) throw new EntityNotFoundException(string.Format(_localizer["notification.notfound"], id));

        toUpdate = toUpdate.Update(true);
        await _repository.UpdateAsync(toUpdate);

        toUpdate.DomainEvents.Add(new NotificationUpdatedEvent(toUpdate));

        int result = await _repository.SaveChangesAsync();
        return await Result<bool>.SuccessAsync(result > 0);
    }

    public async Task<Result<bool>> ReadNotificationAsync(List<Guid> ids)
    {
        if (ids == null) return await Result<bool>.SuccessAsync(true);

        var notificationsToUpdate = await _repository.FindByConditionAsync<Notification>(x => ids.Contains(x.Id));

        foreach (var notification in notificationsToUpdate)
        {
            notification.Update(true);
            await _repository.UpdateAsync(notification);
            notification.DomainEvents.Add(new NotificationUpdatedEvent(notification));
        }

        int result = await _repository.SaveChangesAsync();
        return await Result<bool>.SuccessAsync(result > 0);
    }

    public async Task<Result<bool>> UnReadNotificationAsync(Guid id)
    {
        var toUpdate = await _repository.GetByIdAsync<Notification>(id);
        if (toUpdate == null) throw new EntityNotFoundException(string.Format(_localizer["notification.notfound"], id));

        toUpdate = toUpdate.Update(false);
        await _repository.UpdateAsync(toUpdate);

        toUpdate.DomainEvents.Add(new NotificationUpdatedEvent(toUpdate));

        int result = await _repository.SaveChangesAsync();
        return await Result<bool>.SuccessAsync(result > 0);
    }

}