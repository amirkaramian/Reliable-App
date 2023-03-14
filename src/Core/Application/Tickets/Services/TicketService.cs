using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Tickets.Interfaces;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Tickets;
using MyReliableSite.Shared.DTOs.Tickets;
using MyReliableSite.Application.Specifications;
using Mapster;
using MyReliableSite.Shared.DTOs.Notifications;
using MyReliableSite.Shared.DTOs.Notifications.Enums;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Domain.Tickets.Events;
using MyReliableSite.Application.Storage;
using MyReliableSite.Application.WebHooks.Interfaces;
using MyReliableSite.Shared.DTOs.Identity;
using MyReliableSite.Domain.Departments;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Shared.DTOs.Departments;
using MyReliableSite.Shared.DTOs.Brands;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Products;
using MyReliableSite.Shared.DTOs.EmailTemplates;
using MyReliableSite.Application.SmtpConfigurations.Interfaces;
using MyReliableSite.Shared.DTOs.Transaction;
using MyReliableSite.Shared.DTOs.Products;

using System;
using MyReliableSite.Domain.ArticleFeedbacks;
using MyReliableSite.Application.Multitenancy;
using MyReliableSite.Domain.Multitenancy;

namespace MyReliableSite.Application.Tickets.Services;

public class TicketService : ITicketService
{
    private readonly IStringLocalizer<TicketService> _localizer;
    private readonly IRepositoryAsync _repository;
    private readonly IUserService _userService;
    private readonly IFileStorageService _fileStorageService;
    private readonly INotificationService _notificationService;
    private readonly ICurrentUser _user;
    private readonly IJobService _jobService;
    private readonly IWebHooksSenderService _webHooksSenderService;
    private readonly IMailService _mailService;
    private readonly ITenantService _tenantService;
    public TicketService()
    {
    }

    public TicketService(IRepositoryAsync repository, ICurrentUser user, IJobService jobService, IWebHooksSenderService webHooksSenderService, IUserService userService, IFileStorageService fileStorageService, IStringLocalizer<TicketService> localizer, INotificationService notificationService, IMailService mailService, ITenantService tenantService)
    {
        _repository = repository;
        _userService = userService;
        _fileStorageService = fileStorageService;
        _localizer = localizer;
        _notificationService = notificationService;
        _user = user;
        _jobService = jobService;
        _webHooksSenderService = webHooksSenderService;
        _mailService = mailService;
        _tenantService = tenantService;
    }

    public async Task<Result<Guid>> CreateTicketAsync(CreateTicketRequest request)
    {
        string clientEmail = string.Empty, clientFullName = string.Empty, brandId = string.Empty;
        var user = new UserDetailsDto();
        if (!string.IsNullOrEmpty(request.ClientId))
        {
            var clientUser = await _userService.GetAsync(request.ClientId);
            if (clientUser?.Data == null) return await Result<Guid>.SuccessAsync(Guid.Empty);
            clientEmail = clientUser.Data.Email;
            clientFullName = clientUser.Data.FullName;
            brandId = clientUser.Data.BrandId;
            user = clientUser?.Data;
        }

        var ticket = new Ticket(
            request.TicketTitle,
            request.Description,
            request.AssignedTo,
            request.TicketPriority,
            request.TicketRelatedTo,
            request.TicketRelatedToId,
            request.TicketStatus,
            request.DepartmentId,
            request.Duration,
            request.IdleTime,
            brandId,
            clientFullName,
            request.PinTicket,
            clientEmail,
            request.FollowUpOn,
            request.FollowUpComment,
            request.Group,
            request.AgentUser,
            request.PriorityFollowUp,
            request.Notes,
            request.TransferComments,
            request.TransferOn,
            request.ClientId,
            request.IncomingFromClient);
        ticket.DomainEvents.Add(new TicketCreatedEvent(ticket));
        var ticketId = await _repository.CreateAsync<Ticket>((Ticket)ticket);
        await _repository.SaveChangesAsync();

        string message = $"Hello [[fullName]], a new ticket has been created.";
        if (string.IsNullOrEmpty(request.Group))
        {
            var usersAdminGroup = await _userService.GetAllUsersofAdminGroupAsync(request.Group);
            if (usersAdminGroup.Data != null)
            {
                await _notificationService.SendMessageToUsersAsync(usersAdminGroup.Data.Select(x => x.Id.ToString()), new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.TICKET_CREATED, TargetUserTypes = NotificationTargetUserTypes.SuperAdmins, NotifyModelId = ticket.Id, Data = new { TicketId = ticket.Id } });
            }
        }

        await _notificationService.SendMessageToSuperAdminsHavingPermissionAsync(new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.TICKET_CREATED, TargetUserTypes = NotificationTargetUserTypes.SuperAdmins, NotifyModelId = ticket.Id, Data = new { TicketId = ticket.Id } });

        message = $"Hello [[fullName]], a new ticket has been created of you.";
        await sendNotificationToUser(request.ClientId, ticket, message);

        message = $"Hello [[fullName]], a new ticket has been assigned to you.";
        await sendNotificationToUser(request.AgentUser, ticket, message);
        await sendNotificationToUser(request.AssignedTo, ticket, message);

        // send email to client when ticket create
        string createTicketUrl = $"https://admin.myreliablesite.m2mbeta.com/admin/dashboard/support/tickets/show-all/list/details/{ticketId}?tid={ticketId}";
        await _mailService.SendEmailViaSMTPTemplate(new List<UserDetailsDto> { user }, EmailTemplateType.TicketCreated, ticket.TicketTitle, null, createTicketUrl);

        if (!string.IsNullOrWhiteSpace(request.AssignedTo))
        {
            var assignedUserData = await _userService.GetAsync(request.AssignedTo);
            var assignedUser = assignedUserData.Data;
            await _mailService.SendEmailViaSMTPTemplate(new List<UserDetailsDto> { assignedUser }, EmailTemplateType.TicketAssignment, ticket.TicketTitle, null, createTicketUrl);
        }

        _jobService.Enqueue(() => _webHooksSenderService.sendWebHook<Ticket>(ticket, "Ticket"));

        // _jobService.Schedule(() => TicketAssignmentRoundRibionAsync(ticket), TimeSpan.FromMinutes(1));
        await createTicketHistoryAsync(ticket);
        return await Result<Guid>.SuccessAsync(ticketId);
    }

    private async Task<bool> TicketAssignmentRoundRibionAsync(Ticket ticket)
    {
        string message = $"Hello [[fullName]], a ticket has been updated.";
        if (string.IsNullOrEmpty(ticket.Group))
        {

            var usersAdminGroup = await _userService.GetAllUsersofAdminGroupAsync(ticket.Group);
            if (usersAdminGroup.Data != null)
            {
                var userAdminIds = usersAdminGroup.Data.Select(x => x.Id.ToString());

                await _notificationService.SendMessageToUsersAsync(userAdminIds, new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.TICKET_UPDATED, TargetUserTypes = NotificationTargetUserTypes.SuperAdmins, NotifyModelId = ticket.Id, Data = new { TicketId = ticket.Id } });

                var tickets = await _repository.GetListAsync<Ticket>(
               m => userAdminIds.Contains(m.AssignedTo) && (m.TicketStatus == TicketStatus.Active || m.TicketStatus == TicketStatus.Waiting));

                var result = tickets
                    .GroupBy(m => m.AssignedTo)
                    .Select(x => new { AssignedUserId = x.Key, Count = x.Count() })
                    .OrderBy(m => m.Count);

                foreach (var item in result)
                {
                    if (CurrentOnlineUser.OnlineUsers.FirstOrDefault(m => m.UserId == item.AssignedUserId) != null)
                    {
                        ticket.AssignedTo = item.AssignedUserId;
                        message = $"Hello [[fullName]], a ticket is updated which is assigned to you.";
                        await sendNotificationOfUpdateToUser(ticket.AssignedTo, ticket, message);
                        ticket.CreatedBy = Guid.Empty;
                        await _repository.UpdateAsync<Ticket>(ticket);
                        await _repository.SaveChangesAsync();
                        await createTicketHistoryAsync(ticket);

                        break;
                    }
                }
            }
        }

        return true;
    }

    private async Task sendNotificationToUser(string userId, Ticket ticket, string message)
    {
        if (!string.IsNullOrEmpty(userId))
        {
            var client = await _userService.GetUserProfileAsync(userId);
            if (client.Data != null)
            {
                await _notificationService.SendMessageToUserAsync(client.Data.Id.ToString(), new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.TICKET_CREATED, TargetUserTypes = NotificationTargetUserTypes.Admins, NotifyModelId = ticket.Id, Data = new { TicketId = ticket.Id } });
            }
        }
    }

    private async Task sendNotificationOfUpdateToUser(string userId, Ticket ticket, string message)
    {
        if (!string.IsNullOrEmpty(userId))
        {
            var client = await _userService.GetUserProfileAsync(userId);
            if (client.Data != null)
            {
                await _notificationService.SendMessageToUserAsync(ticket.AssignedTo, new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.TICKET_UPDATED, TargetUserTypes = NotificationTargetUserTypes.Admins, NotifyModelId = ticket.Id, Data = new { TicketId = ticket.Id } });
            }
        }
    }

    public async Task<Result<Guid>> DeleteTicketAsync(Guid id)
    {
        var ticket = await _repository.GetByIdAsync<Ticket>(id);
        if (ticket == null) throw new EntityNotFoundException(string.Format(_localizer["Ticket.notfound"], id));

        ticket.DeletedOn = DateTime.UtcNow;
        ticket.DeletedBy = _user.GetUserId();
        ticket.DomainEvents.Add(new TicketDeletedEvent(ticket));

        await _repository.SaveChangesAsync();
        await createTicketHistoryAsync(ticket);

        // user assignment to default ticket
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<PaginatedResult<TicketDto>> SearchTicketsAsync(TicketListFilter filter)
    {

        if (filter.TicketRelatedTo == null)
        {
            return await _repository.GetSearchResultsAsync<Ticket, TicketDto>(
                filter.PageNumber,
                filter.PageSize,
                filter.OrderBy,
                filter.OrderType,
                filter.AdvancedSearch,
                filter.Keyword,
                null);

        }

        return await _repository.GetSearchResultsAsync<Ticket, TicketDto>(
            filter.PageNumber,
            filter.PageSize,
            filter.OrderBy,
            filter.OrderType,
            filter.AdvancedSearch,
            filter.Keyword,
            m => m.TicketRelatedTo == (TicketRelatedTo)filter.TicketRelatedTo);
    }

    public async Task<PaginatedResult<TicketDto>> SearchAsync(TicketListFilter filter)
    {
        var filters = new Filters<Ticket>();
        filters.Add(filter.TicketStatus.HasValue, x => x.TicketStatus == (TicketStatus)filter.TicketStatus);
        filters.Add(filter.TicketPriority.HasValue, x => x.TicketPriority == (TicketPriority)filter.TicketPriority);
        filters.Add(filter.TicketRelatedTo.HasValue, x => x.TicketRelatedTo == (TicketRelatedTo)filter.TicketRelatedTo);
        filters.Add(filter.StartDate.HasValue && filter.EndDate.HasValue, x => x.CreatedOn.Date >= filter.StartDate.Value.Date && x.CreatedOn <= filter.EndDate.Value.Date);
        filters.Add(filter.StartDate.HasValue && !filter.EndDate.HasValue, x => x.CreatedOn.Date >= filter.StartDate.Value.Date);
        filters.Add(!filter.StartDate.HasValue && filter.EndDate.HasValue, x => x.CreatedOn.Date <= filter.EndDate.Value.Date);

        filters.Add(!string.IsNullOrEmpty(filter.AssignedTo), x => x.AssignedTo == filter.AssignedTo);
        filters.Add(filter.TicketNumber.HasValue, x => x.TicketNumber == filter.TicketNumber);

        if (!string.IsNullOrEmpty(filter.ClientEmail))
        {
            var userEmail = await _userService.GetUserProfileByEmailAsync(filter.ClientEmail);
            if (userEmail != null && userEmail.Data != null)
            {
                filters.Add(!string.IsNullOrEmpty(filter.ClientEmail), x => x.ClientId == userEmail.Data.Id.ToString());

            }
        }

        if (!string.IsNullOrEmpty(filter.ClientId))
        {
            var userName = await _userService.GetAsync(filter.ClientId);
            if (userName != null && userName.Data != null)
            {
                filters.Add(!string.IsNullOrEmpty(filter.ClientId), x => x.ClientId == userName.Data.Id.ToString());

            }
        }

        if (!string.IsNullOrEmpty(filter.DepartmentId))
        {
            filters.Add(!string.IsNullOrEmpty(filter.DepartmentId), x => x.DepartmentId == filter.DepartmentId);
        }

        var resultset = await _repository.GetSearchResultsAsync<Ticket, TicketDto>(
            filter.PageNumber,
            filter.PageSize,
            filter.OrderBy,
            filter.OrderType,
            filters,
            filter.AdvancedSearch,
            filter.Keyword);

        foreach (var item in resultset.Data)
        {
            item.TicketCommentsCount = await _repository.CountByConditionAsync<TicketComment>(m => m.Ticket.Id == item.Id);
        }

        return resultset;
    }

    public async Task<Result<Guid>> UpdateTicketAsync(UpdateTicketRequest request, Guid id)
    {
        var ticket = await _repository.GetByIdAsync<Ticket>(id);
        if (ticket == null) throw new EntityNotFoundException(string.Format(_localizer["Ticket.notfound"], id));

        var updatedTicket = ticket.Update(
            request.TicketTitle,
            request.Description,
            request.AssignedTo,
            request.TicketPriority,
            request.TicketRelatedTo,
            request.TicketRelatedToId,
            request.TicketStatus,
            request.DepartmentId,
            request.Duration,
            request.IdleTime,
            request.BrandId,
            request.PinTicket,
            request.FollowUpOn,
            request.FollowUpComment,
            request.Group,
            request.AgentUser,
            request.PriorityFollowUp,
            request.Notes,
            request.TransferComments,
            request.TransferOn,
            request.IncomingFromClient);
        updatedTicket.DomainEvents.Add(new TicketUpdatedEvent(updatedTicket));
        await _repository.UpdateAsync<Ticket>(updatedTicket);
        await _repository.SaveChangesAsync();

        string message = $"Hello [[fullName]], a ticket has been updated.";
        if (!string.IsNullOrEmpty(ticket.Group))
        {
            var usersAdminGroup = await _userService.GetAllUsersofAdminGroupAsync(ticket.Group);
            if (usersAdminGroup.Data != null)
            {
                await _notificationService.SendMessageToUsersAsync(usersAdminGroup.Data.Select(x => x.Id.ToString()), new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.TICKET_UPDATED, TargetUserTypes = NotificationTargetUserTypes.SuperAdmins, NotifyModelId = ticket.Id, Data = new { TicketId = ticket.Id } });
            }
        }

        await _notificationService.SendMessageToSuperAdminsHavingPermissionAsync(new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.TICKET_UPDATED, TargetUserTypes = NotificationTargetUserTypes.SuperAdmins, NotifyModelId = ticket.Id, Data = new { TicketId = ticket.Id } });

        message = $"Hello [[fullName]], a ticket has been updated of you.";
        await sendNotificationOfUpdateToUser(ticket.ClientId, ticket, message);

        message = $"Hello [[fullName]], a ticket is updated which is assigned to you.";
        await sendNotificationOfUpdateToUser(ticket.AgentUser, ticket, message);
        await sendNotificationOfUpdateToUser(ticket.AssignedTo, ticket, message);

        await createTicketHistoryAsync(ticket);
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> TicketTransferAsync(UpdateTicketRequest request, Guid id)
    {
        var ticket = await _repository.GetByIdAsync<Ticket>(id);
        if (ticket == null) throw new EntityNotFoundException(string.Format(_localizer["Ticket.notfound"], id));

        var updatedTicket = ticket.Update(
            request.TicketTitle,
            request.Description,
            request.AssignedTo,
            request.TicketPriority,
            request.TicketRelatedTo,
            request.TicketRelatedToId,
            request.TicketStatus,
            request.DepartmentId,
            request.Duration,
            request.IdleTime,
            request.BrandId,
            request.PinTicket,
            request.FollowUpOn,
            request.FollowUpComment,
            request.Group,
            request.AgentUser,
            request.PriorityFollowUp,
            request.Notes,
            request.TransferComments,
            request.TransferOn,
            request.IncomingFromClient);
        updatedTicket.DomainEvents.Add(new TicketUpdatedEvent(updatedTicket));
        await _repository.UpdateAsync<Ticket>(updatedTicket);
        await _repository.SaveChangesAsync();

        string message = $"Hello [[fullName]], a ticket has been updated.";
        if (string.IsNullOrEmpty(ticket.Group))
        {
            var usersAdminGroup = await _userService.GetAllUsersofAdminGroupAsync(ticket.Group);
            if (usersAdminGroup.Data != null)
            {
                await _notificationService.SendMessageToUsersAsync(usersAdminGroup.Data.Select(x => x.Id.ToString()), new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.TICKET_UPDATED, TargetUserTypes = NotificationTargetUserTypes.SuperAdmins, NotifyModelId = ticket.Id, Data = new { TicketId = ticket.Id } });
            }
        }

        await _notificationService.SendMessageToSuperAdminsHavingPermissionAsync(new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.TICKET_UPDATED, TargetUserTypes = NotificationTargetUserTypes.SuperAdmins, NotifyModelId = ticket.Id, Data = new { TicketId = ticket.Id } });

        message = $"Hello [[fullName]], a ticket has been updated of you.";
        await sendNotificationOfUpdateToUser(ticket.ClientId, ticket, message);

        message = $"Hello [[fullName]], a ticket is updated which is assigned to you.";
        await sendNotificationOfUpdateToUser(ticket.AgentUser, ticket, message);
        await sendNotificationOfUpdateToUser(ticket.AssignedTo, ticket, message);
        await createTicketHistoryAsync(ticket);

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<TicketDto>> GetClientTicketAsync(Guid id)
    {
        return await GetTicketAsync(id, _user.GetUserId().ToString());
    }

    public async Task<Result<TicketDto>> GetTicketAsync(Guid id, string clientId = "")
    {
        var spec = new BaseSpecification<Ticket>();

        // spec.Includes.Add(a => a.TicketComments);

        spec.IncludeStrings.Add("TicketComments.TicketCommentReplies");
        spec.IncludeStrings.Add("Department");
        spec.IncludeStrings.Add("Brand");
        if (!string.IsNullOrEmpty(clientId))
            spec.And(x => x.AssignedTo == clientId);
        var ticket = await _repository.GetByIdAsync<Ticket, TicketDto>(id, spec);

        if (!string.IsNullOrEmpty(ticket.BrandId))
        {
            ticket.Brand = await _repository.GetByIdAsync<Brand, BrandDto>(Guid.Parse(ticket.BrandId));

        }

        if (!string.IsNullOrEmpty(ticket.DepartmentId))
        {
            ticket.Department = await _repository.GetByIdAsync<Department, DepartmentDto>(Guid.Parse(ticket.DepartmentId));
        }

        ticket.TicketComments.OrderByDescending(m => m.CreatedOn);

        var userIds = ticket.TicketComments.Select(c => c.UserId).ToList();
        foreach (var item in ticket.TicketComments.Select(c => c.TicketCommentReplies.Select(m => m.UserId)).ToList())
        {
            userIds.AddRange(item);
        }

        if (ticket.CreatedBy != Guid.Empty)
        {
            userIds.Add(ticket.CreatedBy.ToString());

        }

        if (!string.IsNullOrEmpty(ticket.AssignedTo))
        {
            userIds.Add(ticket.AssignedTo);

        }

        var userDetails = await _userService.GetAllAsync(userIds);

        if (!string.IsNullOrEmpty(ticket.AssignedTo))
        {
            var assignedToUserObj = userDetails.Data.FirstOrDefault(m => m.Id == Guid.Parse(ticket.AssignedTo));
            if (assignedToUserObj != null)
            {
                ticket.AssignedToFullName = assignedToUserObj.FullName;
            }
        }

        if (ticket.CreatedBy != Guid.Empty)
        {
            var createdByObj = userDetails.Data.FirstOrDefault(m => m.Id == ticket.CreatedBy);
            if (createdByObj != null)
            {
                ticket.CreatedByName = createdByObj.FullName;
            }
        }

        foreach (var itemComment in ticket.TicketComments)
        {

            if (!string.IsNullOrEmpty(itemComment.UserId))
            {
                var commenterObj = userDetails.Data.FirstOrDefault(m => m.Id == Guid.Parse(itemComment.UserId));
                if (commenterObj != null)
                {
                    itemComment.UserFullName = commenterObj.FullName;
                    if (!string.IsNullOrEmpty(commenterObj.ImageUrl))
                    {
                        itemComment.UserImagePath = await _fileStorageService.ReturnBase64StringOfImageFileAsync(commenterObj.ImageUrl);
                    }
                }
            }

            foreach (var itemReply in itemComment.TicketCommentReplies)
            {
                if (!string.IsNullOrEmpty(itemReply.UserId))
                {
                    var itemReplyObj = userDetails.Data.FirstOrDefault(m => m.Id == Guid.Parse(itemReply.UserId));
                    if (itemReplyObj != null)
                    {
                        itemReply.UserFullName = itemReplyObj.FullName;
                        if (!string.IsNullOrEmpty(itemReplyObj.ImageUrl))
                        {
                            itemReply.UserImagePath = await _fileStorageService.ReturnBase64StringOfImageFileAsync(itemReplyObj.ImageUrl);
                        }
                    }
                }
            }

            itemComment.TicketCommentReplies.OrderByDescending(m => m.CreatedOn);
        }

        ticket.TicketCommentsCount = ticket.TicketComments.Count();

        return await Result<TicketDto>.SuccessAsync(ticket);
    }

    public async Task<Result<TicketDto>> GetTicketAsyncWithCommentAndReplies(Guid id)
    {
        var spec = new BaseSpecification<Ticket>();

        string tenant = _user.GetTenant();
        if (tenant.ToLower() == "client")
        {
            spec.And(x => x.AssignedTo == _user.GetUserId().ToString());
        }

        var ticket = await _repository.GetByIdAsync<Ticket, TicketDto>(id, spec);

        if (!string.IsNullOrEmpty(ticket.BrandId))
        {
            ticket.Brand = await _repository.GetByIdAsync<Brand, BrandDto>(Guid.Parse(ticket.BrandId));

        }

        if (!string.IsNullOrEmpty(ticket.DepartmentId))
        {
            ticket.Department = await _repository.GetByIdAsync<Department, DepartmentDto>(Guid.Parse(ticket.DepartmentId));
        }

        var commentList = await _repository.GetListAsync<TicketComment>(m => m.DeletedOn == null && m.TicketId == ticket.Id);
        ticket.TicketComments = new List<TicketCommentDto>();
        foreach (TicketComment x in commentList)
        {
            TicketCommentDto ticketCommentDto = new TicketCommentDto();
            ticketCommentDto.Id = x.Id;
            ticketCommentDto.TicketId = x.TicketId.ToString();
            ticketCommentDto.CommentText = x.CommentText;
            ticketCommentDto.UserId = x.UserId;
            ticketCommentDto.IsSticky = x.IsSticky;
            ticketCommentDto.IsDraft = x.IsDraft;
            ticketCommentDto.TicketCommentAction = x.TicketCommentAction;
            ticketCommentDto.TicketCommentType = x.TicketCommentType;
            ticketCommentDto.CreatedBy = x.CreatedBy;
            ticketCommentDto.CreatedOn = x.CreatedOn;
            ticketCommentDto.LastModifiedBy = x.LastModifiedBy;
            ticketCommentDto.LastModifiedOn = x.LastModifiedOn;
            ticket.TicketComments.Add(ticketCommentDto);

        }

        var userIds = ticket.TicketComments.Select(c => c.UserId).ToList();

        if (ticket.CreatedBy != Guid.Empty)
        {
            userIds.Add(ticket.CreatedBy.ToString());

        }

        if (!string.IsNullOrEmpty(ticket.AssignedTo))
        {
            userIds.Add(ticket.AssignedTo);

        }

        var userDetails = await _userService.GetAllAsync(userIds);

        if (!string.IsNullOrEmpty(ticket.AssignedTo))
        {
            var assignedToUserObj = userDetails.Data.FirstOrDefault(m => m.Id == Guid.Parse(ticket.AssignedTo));
            if (assignedToUserObj != null)
            {
                ticket.AssignedToFullName = assignedToUserObj.FullName;
            }
        }

        if (ticket.CreatedBy != Guid.Empty)
        {
            var createdByObj = userDetails.Data.FirstOrDefault(m => m.Id == ticket.CreatedBy);
            if (createdByObj != null)
            {
                ticket.CreatedByName = createdByObj.FullName;
            }
        }

        foreach (var itemComment in ticket.TicketComments)
        {

            if (!string.IsNullOrEmpty(itemComment.UserId))
            {
                var commentRepliesList = await _repository.GetListAsync<TicketCommentReply>(m => m.DeletedOn == null && m.TicketCommentId == itemComment.Id);

                itemComment.TicketCommentReplies = new List<TicketCommentReplyDto>();
                foreach (TicketCommentReply commentReply in commentRepliesList)
                {
                    TicketCommentReplyDto ticketCommentReplyDto = new TicketCommentReplyDto();
                    ticketCommentReplyDto.Id = commentReply.Id;
                    ticketCommentReplyDto.CommentText = commentReply.CommentText;
                    ticketCommentReplyDto.UserId = commentReply.UserId;
                    ticketCommentReplyDto.TicketCommentParentReplyId = commentReply.TicketCommentParentReplyId;
                    ticketCommentReplyDto.CreatedBy = commentReply.CreatedBy;
                    ticketCommentReplyDto.CreatedOn = commentReply.CreatedOn;
                    ticketCommentReplyDto.LastModifiedBy = commentReply.LastModifiedBy;
                    ticketCommentReplyDto.LastModifiedOn = commentReply.LastModifiedOn;

                    var replierObj = userDetails.Data.FirstOrDefault(m => m.Id == Guid.Parse(commentReply.UserId));
                    if (replierObj != null)
                    {
                        ticketCommentReplyDto.UserFullName = replierObj.FullName;
                        if (!string.IsNullOrEmpty(replierObj.ImageUrl))
                        {
                            ticketCommentReplyDto.UserImagePath = await _fileStorageService.ReturnBase64StringOfImageFileAsync(replierObj.ImageUrl);
                        }
                    }

                    itemComment.TicketCommentReplies.Add(ticketCommentReplyDto);
                }

                var commenterObj = userDetails.Data.FirstOrDefault(m => m.Id == Guid.Parse(itemComment.UserId));
                if (commenterObj != null)
                {
                    itemComment.UserFullName = commenterObj.FullName;
                    if (!string.IsNullOrEmpty(commenterObj.ImageUrl))
                    {
                        itemComment.UserImagePath = await _fileStorageService.ReturnBase64StringOfImageFileAsync(commenterObj.ImageUrl);
                    }
                }
            }

        }

        ticket.TicketCommentsCount = ticket.TicketComments.Count();

        return await Result<TicketDto>.SuccessAsync(ticket);
    }

    public async Task<Result<List<TicketDto>>> GetTicketByClientIdAsync(Guid ClientId)
    {
        var ticket = await _repository.GetListAsync<Ticket>(m => m.ClientId == ClientId.ToString());
        var mappedTickets = ticket.Adapt<List<TicketDto>>();
        return await Result<List<TicketDto>>.SuccessAsync(mappedTickets);
    }

    public async Task<Result<List<TicketDto>>> GetCurrentUserTicketsAsync()
    {
        var ticket = await _repository.GetListAsync<Ticket>(m => m.AssignedTo == _user.GetUserId().ToString());
        var mappedTickets = ticket.Adapt<List<TicketDto>>();
        return await Result<List<TicketDto>>.SuccessAsync(mappedTickets);
    }

    public async Task<Result<List<TicketHistoryDto>>> GetTicketHistoryAsync(Guid id)
    {
        var tickets = await _repository.GetListAsync<TicketHistory>(m => m.TicketId == id);

        var userIds = tickets.Select(m => m.ActionBy.ToString()).Distinct().ToList();
        userIds.AddRange(tickets.Select(m => m.AssignedTo).Distinct().ToList());
        var userDetails = await _userService.GetAllAsync(userIds);

        var ticketCommentHistoryIds = tickets.Select(m => m.TicketCommentHistoryId).Distinct().ToList();
        var ticketCommentReplyHistoryIds = tickets.Select(m => m.TicketCommentReplyHistoryId).Distinct().ToList();
        var ticketCommentHistories = await _repository.GetListAsync<TicketCommentHistory>(m => ticketCommentHistoryIds.Contains(m.Id));
        var ticketCommentReplyHistories = await _repository.GetListAsync<TicketCommentReplyHistory>(m => ticketCommentReplyHistoryIds.Contains(m.Id));

        var ticketsDto = tickets.Adapt<List<TicketHistoryDto>>();

        foreach (var ticket in ticketsDto)
        {
            if (ticket.TicketCommentReplyHistoryId != null && ticket.TicketCommentReplyHistoryId != Guid.Empty)
            {
                var ticketCommentReplyHistory = ticketCommentReplyHistories.FirstOrDefault(m => m.Id == ticket.TicketCommentReplyHistoryId);
                if (ticketCommentReplyHistory != null)
                {
                    ticket.TicketCommentReplyHistory = ticketCommentReplyHistory.Adapt<TicketCommentReplyHistoryDto>();
                }
            }

            if (ticket.TicketCommentHistoryId != null && ticket.TicketCommentHistoryId != Guid.Empty)
            {
                var ticketCommentHistory = ticketCommentHistories.FirstOrDefault(m => m.Id == ticket.TicketCommentHistoryId);
                if (ticketCommentHistory != null)
                {
                    ticket.TicketCommentHistory = ticketCommentHistory.Adapt<TicketCommentHistoryDto>();
                }
            }

            if (!string.IsNullOrEmpty(ticket.AssignedTo))
            {
                var assignedToUserObj = userDetails.Data.FirstOrDefault(m => m.Id == Guid.Parse(ticket.AssignedTo));
                if (assignedToUserObj != null)
                {
                    ticket.AssignedToFullName = assignedToUserObj.FullName;
                }
            }

            if (ticket.ActionBy != Guid.Empty)
            {
                var createdByObj = userDetails.Data.FirstOrDefault(m => m.Id == ticket.ActionBy);
                if (createdByObj != null)
                {
                    ticket.ActionByName = createdByObj.FullName;
                }
            }
        }

        return await Result<List<TicketHistoryDto>>.SuccessAsync(ticketsDto);
    }

    private async Task createTicketHistoryAsync(Ticket request)
    {

        var ticketHistory = new TicketHistory(
            request.TicketTitle,
            request.Description,
            request.AssignedTo,
            request.TicketPriority,
            request.TicketRelatedTo,
            request.TicketRelatedToId,
            request.TicketStatus,
            request.DepartmentId,
            request.Duration,
            request.IdleTime,
            request.BrandId,
            request.ClientFullName,
            request.PinTicket,
            request.ClientEmail,
            request.FollowUpOn,
            request.FollowUpComment,
            request.Group,
            request.AgentUser,
            request.PriorityFollowUp,
            request.Notes,
            request.TransferComments,
            request.TransferOn,
            request.ClientId,
            _user.GetUserId(),
            request.Id,
            null,
            null,
            null,
            null,
            request.IncomingFromClient);

        await _repository.CreateAsync<TicketHistory>((TicketHistory)ticketHistory);
        await _repository.SaveChangesAsync();

    }

    public async Task<Result<Guid>> CloseTicketAsync(Guid id)
    {
        var tickets = await _repository.FindByConditionAsync<Ticket>(x => x.Id == id && x.AssignedTo == _user.GetUserId().ToString());
        if (tickets == null || !tickets.Any()) throw new EntityNotFoundException(string.Format(_localizer["Ticket.notfound"], id));
        var ticket = tickets.First();
        ticket.TicketStatus = TicketStatus.Closed;
        await _repository.UpdateAsync<Ticket>(ticket);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(ticket.Id);
    }

    public async Task<Result<List<TicketEXL>>> GetTicketListAsync(string userId, DateTime startDate, DateTime endDate)
    {

        var transactions = await _repository.QueryWithDtoAsync<TicketEXL>($@"SELECT T.*
                                                                                                        FROM Tickets T
                                                                                                        WHERE ((CONVERT(date, [T].[CreatedOn]) >= '{startDate}') AND (CONVERT(date, [T].[CreatedOn]) <= '{endDate}')) and DeletedOn is null and ClientId = '{userId}' ORDER BY T.CreatedOn ASC");
        return await Result<List<TicketEXL>>.SuccessAsync(transactions.ToList());
    }

    public async Task<Result<SupportStatisticsDto>> GetSupportDurationAsync(SupportHistoryFilterDto filter)
    {
        Func<Ticket, bool> func = _ => 1 == 1;

        var tickets = await _repository.GetListAsync<Ticket>(x => x.CreatedOn >= filter.StartDate && x.CreatedOn < filter.EndDate);
        if (!string.IsNullOrEmpty(filter.User))
        {
            tickets = tickets.Where(x => x.CreatedBy.ToString() == filter.User).ToList();
        }

        if (!string.IsNullOrEmpty(filter.Customer))
        {
            tickets = tickets.Where(x => x.ClientId == filter.Customer).ToList();
        }

        if (!string.IsNullOrEmpty(filter.Email))
        {
            var userDetails = await _userService.GetAllAsync(new[] { filter.Email });
            tickets = tickets.Where(x => userDetails.Data.Select(y => y.Id.ToString()).ToList().Contains(x.ClientId)).ToList();
        }

        if (!string.IsNullOrEmpty(filter.Ip))
        {
            var restricteList = await _repository.GetListAsync<UserRestrictedIp>(x => x.RestrictAccessIPAddress == filter.Ip);
            var user = restricteList.FirstOrDefault();
            tickets = tickets.Where(x => x.ClientId == user.UserId).ToList();
        }

        var support = new SupportStatisticsDto()
        {
            StarDate = filter.StartDate.ToString("yyyy-MM-dd"),
            EndDate = filter.EndDate.ToString("yyyy-MM-dd"),
            Support = new List<ReportSupportDto>(),
        };
        switch (filter.ReportType)
        {
            case ReportType.Normal:
                FillReport("Normal", tickets);
                break;
            case ReportType.ByCustomer:
                func = x => x.LastModifiedBy.ToString() == x.ClientId;
                FillReport("Customer", tickets);
                break;
            case ReportType.ByAgent:
                func = x => x.LastModifiedBy.ToString() == x.AssignedTo;
                FillReport("Agent", tickets);
                break;
            case ReportType.ByEmail:
                func = x => x.CreatedBy.ToString() == x.AssignedTo;
                FillReport("Email", tickets);
                break;
            case ReportType.LinkedIp:
                var restricted = await _repository.GetListAsync<UserRestrictedIp>(null);
                var userList = restricted.Select(x => x.UserId).ToList();
                func = x => userList.Contains(x.CreatedBy.ToString());
                FillReport("LinkedIp", tickets);
                break;
            case ReportType.Status:
                FillReportByStatus("Status", tickets);
                break;
            case ReportType.Department:
                var depts = tickets.Select(y => y.DepartmentId).Distinct().ToList();
                var departments = await _repository.GetListAsync<Department>(x => depts.Contains(x.Id.ToString()));
                FillReportDepartment("Department", tickets, departments);
                break;
            default:
                FillReport("All", tickets);
                break;
        }

        return await Result<SupportStatisticsDto>.SuccessAsync(support);

        void FillReport(string reportName, IEnumerable<Ticket> tickets)
        {
            var ticketTimes = tickets.Where(x => func.Invoke(x)).Select(x => new
            {
                x.CreatedOn,
                x.LastModifiedOn.Value.Subtract(x.CreatedOn).TotalHours
            }).ToList();
            support.Support.Add(new ReportSupportDto()
            {
                ReportBy = reportName,
                FilterBy = "Hourly",
                Details = (from ti in ticketTimes
                           group ti by new { Hourly = ti.CreatedOn.ToString("HH") } into grp
                           select new SupportDetailDto
                           {
                               Hourly = grp.Key.Hourly,
                               LongTime = (int)grp.Sum(x => x.TotalHours)
                           }).ToList(),

            });
            support.Support.Add(new ReportSupportDto()
            {
                ReportBy = reportName,
                FilterBy = "Day",
                Details = (from ti in ticketTimes
                           group ti by new { Day = ti.CreatedOn.ToString("dd") } into grp
                           select new SupportDetailDto
                           {
                               Day = grp.Key.Day,
                               LongTime = (int)grp.Sum(x => x.TotalHours)
                           }).ToList(),

            });
            support.Support.Add(new ReportSupportDto()
            {
                ReportBy = reportName,
                FilterBy = "Month",
                Details = (from ti in ticketTimes
                           group ti by new { Month = ti.CreatedOn.ToString("MMMM") } into grp
                           select new SupportDetailDto
                           {
                               Month = grp.Key.Month,
                               LongTime = (int)grp.Sum(x => x.TotalHours)
                           }).ToList(),

            });
            support.Support.Add(new ReportSupportDto()
            {
                ReportBy = reportName,
                FilterBy = "Year",
                Details = (from ti in ticketTimes
                           group ti by new { Year = ti.CreatedOn.ToString("yyyy") } into grp
                           select new SupportDetailDto
                           {
                               Year = grp.Key.Year,
                               LongTime = (int)grp.Sum(x => x.TotalHours)
                           }).ToList(),

            });
        }

        void FillReportByStatus(string reportName, IEnumerable<Ticket> tickets)
        {
            var ticketTimes = tickets.Select(x => new
            {
                x.CreatedOn,
                x.LastModifiedOn.Value.Subtract(x.CreatedOn).TotalHours,
                x.TicketStatus,
            }).ToList();
            var list3 = (from ti in ticketTimes
                         group ti by new { Status = ti.TicketStatus, Hourly = ti.CreatedOn.ToString("HH") } into grp
                         select new
                         {
                             Hourly = grp.Key.Hourly,
                             LongTime = (int)grp.Sum(x => x.TotalHours),
                             Status = grp.Key.Status.ToString()
                         }).ToList();
            var statuslist = list3.Select(x => x.Status).Distinct().ToList();
            foreach (string item in statuslist)
            {
                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Hourly",
                    Status = item,
                    Details = list3.Where(x => x.Status == item).Select(x => new SupportDetailDto()
                    {
                        Hourly = x.Hourly,
                        LongTime = x.LongTime,
                    }).ToList()

                });
            }

            var list = (from ti in ticketTimes
                        group ti by new { Status = ti.TicketStatus, Day = ti.CreatedOn.ToString("dd") } into grp
                        select new
                        {
                            Day = grp.Key.Day,
                            LongTime = (int)grp.Sum(x => x.TotalHours),
                            Status = grp.Key.Status.ToString()
                        }).ToList();
            statuslist = list.Select(x => x.Status).Distinct().ToList();
            foreach (string item in statuslist)
            {
                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Day",
                    Status = item,
                    Details = list.Where(x => x.Status == item).Select(x => new SupportDetailDto()
                    {
                        Day = x.Day,
                        LongTime = x.LongTime,
                    }).ToList()

                });
            }

            var list1 = (from ti in ticketTimes
                         group ti by new { Status = ti.TicketStatus, Month = ti.CreatedOn.ToString("MMMM") } into grp
                         select new
                         {
                             Month = grp.Key.Month,
                             LongTime = (int)grp.Sum(x => x.TotalHours),
                             Status = grp.Key.Status.ToString()
                         }).ToList();
            statuslist = list1.Select(x => x.Status).Distinct().ToList();
            foreach (string item in statuslist)
            {
                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Month",
                    Status = item,
                    Details = list1.Where(x => x.Status == item).Select(x => new SupportDetailDto()
                    {
                        Month = x.Month,
                        LongTime = x.LongTime,
                    }).ToList()

                });
            }

            var list2 = (from ti in ticketTimes
                         group ti by new { Status = ti.TicketStatus, Year = ti.CreatedOn.ToString("yyyy") } into grp
                         select new
                         {
                             Year = grp.Key.Year,
                             LongTime = (int)grp.Sum(x => x.TotalHours),
                             Status = grp.Key.Status.ToString()
                         }).ToList();
            statuslist = list2.Select(x => x.Status).Distinct().ToList();
            foreach (string item in statuslist)
            {

                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Year",
                    Status = item,
                    Details = list2.Where(x => x.Status == item).Select(x => new SupportDetailDto()
                    {
                        Year = x.Year,
                        LongTime = x.LongTime,
                    }).ToList()
                });
            }
        }

        void FillReportDepartment(string reportName, IEnumerable<Ticket> tickets, List<Department> departments)
        {
            var ticketTimes = tickets.Select(x => new
            {
                x.CreatedOn,
                x.LastModifiedOn.Value.Subtract(x.CreatedOn).TotalHours,
                x.DepartmentId,
            }).ToList();

            var list = (from ti in ticketTimes
                        group ti by new { Department = ti.DepartmentId, Hourly = ti.CreatedOn.ToString("HH") } into grp
                        select new
                        {
                            Hourly = grp.Key.Hourly,
                            LongTime = (int)grp.Sum(x => x.TotalHours),
                            Department = departments.FirstOrDefault(x => x.Id.ToString() == grp.Key.Department).Name
                        }).ToList();
            var distictList = list.Select(x => x.Department).Distinct().ToList();
            foreach (string item in distictList)
            {
                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Hourly",
                    Department = item,
                    Details = (from ti in ticketTimes
                               group ti by new { Department = ti.DepartmentId, Hourly = ti.CreatedOn.ToString("HH") } into grp
                               select new
                               {
                                   Hourly = grp.Key.Hourly,
                                   LongTime = (int)grp.Sum(x => x.TotalHours),
                                   Department = departments.FirstOrDefault(x => x.Id.ToString() == grp.Key.Department).Name
                               }).ToList().Where(x => x.Department == item).Select(x => new SupportDetailDto()
                               {
                                   Hourly = x.Hourly,
                                   LongTime = x.LongTime,
                               }).ToList()

                });
            }

            var list1 = (from ti in ticketTimes
                         group ti by new { Department = ti.DepartmentId, Day = ti.CreatedOn.ToString("dd") } into grp
                         select new
                         {
                             Day = grp.Key.Day,
                             LongTime = (int)grp.Sum(x => x.TotalHours),
                             Department = departments.FirstOrDefault(x => x.Id.ToString() == grp.Key.Department).Name
                         }).ToList();
            distictList = list.Select(x => x.Department).Distinct().ToList();
            foreach (string item in distictList)
            {
                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Day",
                    Department = item,
                    Details = list1.Where(x => x.Department == item).Select(x => new SupportDetailDto()
                    {
                        Day = x.Day,
                        LongTime = x.LongTime,
                    }).ToList()

                });
            }

            var list2 = (from ti in ticketTimes
                         group ti by new { Department = ti.DepartmentId, Month = ti.CreatedOn.ToString("MMMM") } into grp
                         select new
                         {
                             Month = grp.Key.Month,
                             LongTime = (int)grp.Sum(x => x.TotalHours),
                             Department = departments.FirstOrDefault(x => x.Id.ToString() == grp.Key.Department).Name
                         }).ToList();
            distictList = list.Select(x => x.Department).Distinct().ToList();
            foreach (string item in distictList)
            {
                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Month",
                    Department = item,
                    Details = list2.Where(x => x.Department == item).Select(x => new SupportDetailDto()
                    {
                        Month = x.Month,
                        LongTime = x.LongTime,
                    }).ToList()

                });
            }

            var list3 = (from ti in ticketTimes
                         group ti by new { Department = ti.DepartmentId, Year = ti.CreatedOn.ToString("yyyy") } into grp
                         select new
                         {
                             Year = grp.Key.Year,
                             LongTime = (int)grp.Sum(x => x.TotalHours),
                             Department = departments.FirstOrDefault(x => x.Id.ToString() == grp.Key.Department).Name
                         }).ToList();
            distictList = list.Select(x => x.Department).Distinct().ToList();
            foreach (string item in distictList)
            {
                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Year",
                    Department = item,
                    Details = list3.Where(x => x.Department == item).Select(x => new SupportDetailDto()
                    {
                        Year = x.Year,
                        LongTime = x.LongTime,
                    }).ToList()

                });
            }
        }
    }

    public async Task<Result<SupportStatisticsDto>> GetResponseTimeTicketAsync(SupportHistoryFilterDto filter)
    {
        Func<Ticket, bool> func = _ => 1 == 1;
        var ticketList = await _repository.GetListAsync<Ticket>(x => x.CreatedOn >= filter.StartDate && x.CreatedOn < filter.EndDate);

        var comments = await _repository.GetListAsync<TicketComment>(x => ticketList.Select(y => y.Id).Contains(x.Ticket.Id));

        var ticketIntegrates = (from ti in ticketList
                                join cm in comments
                                on ti.Id equals cm.Ticket.Id
                                into grp
                                from comment in grp.DefaultIfEmpty()
                                select new
                                {
                                    Ticket = ti,
                                    NewLastModifiedOn = comment != null ? comment.CreatedOn : ti.LastModifiedOn,
                                }).ToList();
        var tickets = new List<Ticket>();
        foreach (var item in ticketIntegrates)
        {
            var temp = item.Ticket;
            temp.LastModifiedOn = item.NewLastModifiedOn;
            tickets.Add(temp);
        }

        if (!string.IsNullOrEmpty(filter.User))
        {
            tickets = tickets.Where(x => x.CreatedBy.ToString() == filter.User).ToList();
        }

        if (!string.IsNullOrEmpty(filter.Customer))
        {
            tickets = tickets.Where(x => x.ClientId == filter.Customer).ToList();
        }

        if (!string.IsNullOrEmpty(filter.Email))
        {
            var userDetails = await _userService.GetAllAsync(new[] { filter.Email });
            tickets = tickets.Where(x => userDetails.Data.Select(y => y.Id.ToString()).ToList().Contains(x.ClientId)).ToList();
        }

        if (!string.IsNullOrEmpty(filter.Ip))
        {
            var restricteList = await _repository.GetListAsync<UserRestrictedIp>(x => x.RestrictAccessIPAddress == filter.Ip);
            var user = restricteList.FirstOrDefault();
            tickets = tickets.Where(x => x.ClientId == user.UserId).ToList();
        }

        var support = new SupportStatisticsDto()
        {
            StarDate = filter.StartDate.ToString("yyyy-MM-dd"),
            EndDate = filter.EndDate.ToString("yyyy-MM-dd"),
            Support = new List<ReportSupportDto>(),
        };
        switch (filter.ReportType)
        {
            case ReportType.Normal:
                FillReport("Normal", tickets);
                break;
            case ReportType.ByCustomer:
                func = x => x.LastModifiedBy.ToString() == x.ClientId;
                FillReport("Customer", tickets);
                break;
            case ReportType.ByAgent:
                func = x => x.LastModifiedBy.ToString() == x.AssignedTo;
                FillReport("Agent", tickets);
                break;
            case ReportType.ByEmail:
                func = x => x.CreatedBy.ToString() == x.AssignedTo;
                FillReport("Email", tickets);
                break;
            case ReportType.LinkedIp:
                var restricted = await _repository.GetListAsync<UserRestrictedIp>(null);
                var userList = restricted.Select(x => x.UserId).ToList();
                func = x => userList.Contains(x.CreatedBy.ToString());
                FillReport("LinkedIp", tickets);
                break;
            case ReportType.Status:
                FillReportByStatus("Status", tickets);
                break;
            case ReportType.Department:
                var depts = tickets.Select(y => y.DepartmentId).Distinct().ToList();
                var departments = await _repository.GetListAsync<Department>(x => depts.Contains(x.Id.ToString()));
                FillReportDepartment("Department", tickets, departments);
                break;
            default:
                FillReport("All", tickets);
                break;
        }

        return await Result<SupportStatisticsDto>.SuccessAsync(support);

        void FillReport(string reportName, IEnumerable<Ticket> tickets)
        {
            var ticketTimes = tickets.Where(x => func.Invoke(x)).Select(x => new
            {
                x.CreatedOn,
                x.LastModifiedOn.Value.Subtract(x.CreatedOn).TotalHours
            }).ToList();
            support.Support.Add(new ReportSupportDto()
            {
                ReportBy = reportName,
                FilterBy = "Hourly",
                Details = (from ti in ticketTimes
                           group ti by new { Hourly = ti.CreatedOn.ToString("HH") } into grp
                           select new SupportDetailDto
                           {
                               Hourly = grp.Key.Hourly,
                               LongTime = (int)grp.Sum(x => x.TotalHours)
                           }).ToList(),

            });
            support.Support.Add(new ReportSupportDto()
            {
                ReportBy = reportName,
                FilterBy = "Day",
                Details = (from ti in ticketTimes
                           group ti by new { Day = ti.CreatedOn.ToString("dd") } into grp
                           select new SupportDetailDto
                           {
                               Day = grp.Key.Day,
                               LongTime = (int)grp.Sum(x => x.TotalHours)
                           }).ToList(),

            });
            support.Support.Add(new ReportSupportDto()
            {
                ReportBy = reportName,
                FilterBy = "Month",
                Details = (from ti in ticketTimes
                           group ti by new { Month = ti.CreatedOn.ToString("MMMM") } into grp
                           select new SupportDetailDto
                           {
                               Month = grp.Key.Month,
                               LongTime = (int)grp.Sum(x => x.TotalHours)
                           }).ToList(),

            });
            support.Support.Add(new ReportSupportDto()
            {
                ReportBy = reportName,
                FilterBy = "Year",
                Details = (from ti in ticketTimes
                           group ti by new { Year = ti.CreatedOn.ToString("yyyy") } into grp
                           select new SupportDetailDto
                           {
                               Year = grp.Key.Year,
                               LongTime = (int)grp.Sum(x => x.TotalHours)
                           }).ToList(),

            });
        }

        void FillReportByStatus(string reportName, IEnumerable<Ticket> tickets)
        {
            var ticketTimes = tickets.Select(x => new
            {
                x.CreatedOn,
                x.LastModifiedOn.Value.Subtract(x.CreatedOn).TotalHours,
                x.TicketStatus,
            }).ToList();
            var list3 = (from ti in ticketTimes
                         group ti by new { Status = ti.TicketStatus, Hourly = ti.CreatedOn.ToString("HH") } into grp
                         select new
                         {
                             Hourly = grp.Key.Hourly,
                             LongTime = (int)grp.Sum(x => x.TotalHours),
                             Status = grp.Key.Status.ToString()
                         }).ToList();
            var statuslist = list3.Select(x => x.Status).Distinct().ToList();
            foreach (string item in statuslist)
            {
                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Hourly",
                    Status = item,
                    Details = list3.Where(x => x.Status == item).Select(x => new SupportDetailDto()
                    {
                        Hourly = x.Hourly,
                        LongTime = x.LongTime,
                    }).ToList()

                });
            }

            var list = (from ti in ticketTimes
                        group ti by new { Status = ti.TicketStatus, Day = ti.CreatedOn.ToString("dd") } into grp
                        select new
                        {
                            Day = grp.Key.Day,
                            LongTime = (int)grp.Sum(x => x.TotalHours),
                            Status = grp.Key.Status.ToString()
                        }).ToList();
            statuslist = list.Select(x => x.Status).Distinct().ToList();
            foreach (string item in statuslist)
            {
                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Day",
                    Status = item,
                    Details = list.Where(x => x.Status == item).Select(x => new SupportDetailDto()
                    {
                        Day = x.Day,
                        LongTime = x.LongTime,
                    }).ToList()

                });
            }

            var list1 = (from ti in ticketTimes
                         group ti by new { Status = ti.TicketStatus, Month = ti.CreatedOn.ToString("MMMM") } into grp
                         select new
                         {
                             Month = grp.Key.Month,
                             LongTime = (int)grp.Sum(x => x.TotalHours),
                             Status = grp.Key.Status.ToString()
                         }).ToList();
            statuslist = list1.Select(x => x.Status).Distinct().ToList();
            foreach (string item in statuslist)
            {
                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Month",
                    Status = item,
                    Details = list1.Where(x => x.Status == item).Select(x => new SupportDetailDto()
                    {
                        Month = x.Month,
                        LongTime = x.LongTime,
                    }).ToList()

                });
            }

            var list2 = (from ti in ticketTimes
                         group ti by new { Status = ti.TicketStatus, Year = ti.CreatedOn.ToString("yyyy") } into grp
                         select new
                         {
                             Year = grp.Key.Year,
                             LongTime = (int)grp.Sum(x => x.TotalHours),
                             Status = grp.Key.Status.ToString()
                         }).ToList();
            statuslist = list2.Select(x => x.Status).Distinct().ToList();
            foreach (string item in statuslist)
            {

                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Year",
                    Status = item,
                    Details = list2.Where(x => x.Status == item).Select(x => new SupportDetailDto()
                    {
                        Year = x.Year,
                        LongTime = x.LongTime,
                    }).ToList()
                });
            }
        }

        void FillReportDepartment(string reportName, IEnumerable<Ticket> tickets, List<Department> departments)
        {
            var ticketTimes = tickets.Select(x => new
            {
                x.CreatedOn,
                x.LastModifiedOn.Value.Subtract(x.CreatedOn).TotalHours,
                x.DepartmentId,
            }).ToList();

            var list = (from ti in ticketTimes
                        group ti by new { Department = ti.DepartmentId, Hourly = ti.CreatedOn.ToString("HH") } into grp
                        select new
                        {
                            Hourly = grp.Key.Hourly,
                            LongTime = (int)grp.Sum(x => x.TotalHours),
                            Department = departments.FirstOrDefault(x => x.Id.ToString() == grp.Key.Department).Name
                        }).ToList();
            var distictList = list.Select(x => x.Department).Distinct().ToList();
            foreach (string item in distictList)
            {
                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Hourly",
                    Department = item,
                    Details = (from ti in ticketTimes
                               group ti by new { Department = ti.DepartmentId, Hourly = ti.CreatedOn.ToString("HH") } into grp
                               select new
                               {
                                   Hourly = grp.Key.Hourly,
                                   LongTime = (int)grp.Sum(x => x.TotalHours),
                                   Department = departments.FirstOrDefault(x => x.Id.ToString() == grp.Key.Department).Name
                               }).ToList().Where(x => x.Department == item).Select(x => new SupportDetailDto()
                               {
                                   Hourly = x.Hourly,
                                   LongTime = x.LongTime,
                               }).ToList()

                });
            }

            var list1 = (from ti in ticketTimes
                         group ti by new { Department = ti.DepartmentId, Day = ti.CreatedOn.ToString("dd") } into grp
                         select new
                         {
                             Day = grp.Key.Day,
                             LongTime = (int)grp.Sum(x => x.TotalHours),
                             Department = departments.FirstOrDefault(x => x.Id.ToString() == grp.Key.Department).Name
                         }).ToList();
            distictList = list.Select(x => x.Department).Distinct().ToList();
            foreach (string item in distictList)
            {
                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Day",
                    Department = item,
                    Details = list1.Where(x => x.Department == item).Select(x => new SupportDetailDto()
                    {
                        Day = x.Day,
                        LongTime = x.LongTime,
                    }).ToList()

                });
            }

            var list2 = (from ti in ticketTimes
                         group ti by new { Department = ti.DepartmentId, Month = ti.CreatedOn.ToString("MMMM") } into grp
                         select new
                         {
                             Month = grp.Key.Month,
                             LongTime = (int)grp.Sum(x => x.TotalHours),
                             Department = departments.FirstOrDefault(x => x.Id.ToString() == grp.Key.Department).Name
                         }).ToList();
            distictList = list.Select(x => x.Department).Distinct().ToList();
            foreach (string item in distictList)
            {
                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Month",
                    Department = item,
                    Details = list2.Where(x => x.Department == item).Select(x => new SupportDetailDto()
                    {
                        Month = x.Month,
                        LongTime = x.LongTime,
                    }).ToList()

                });
            }

            var list3 = (from ti in ticketTimes
                         group ti by new { Department = ti.DepartmentId, Year = ti.CreatedOn.ToString("yyyy") } into grp
                         select new
                         {
                             Year = grp.Key.Year,
                             LongTime = (int)grp.Sum(x => x.TotalHours),
                             Department = departments.FirstOrDefault(x => x.Id.ToString() == grp.Key.Department).Name
                         }).ToList();
            distictList = list.Select(x => x.Department).Distinct().ToList();
            foreach (string item in distictList)
            {
                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Year",
                    Department = item,
                    Details = list3.Where(x => x.Department == item).Select(x => new SupportDetailDto()
                    {
                        Year = x.Year,
                        LongTime = x.LongTime,
                    }).ToList()

                });
            }
        }
    }

    public async Task<Result<SupportStatisticsDto>> GetReplyTicketAsync(SupportHistoryFilterDto filter)
    {
        Func<Ticket, bool> func = _ => 1 == 1;
        var tickets = await _repository.GetListAsync<Ticket>(x => x.CreatedOn >= filter.StartDate && x.CreatedOn < filter.EndDate);

        if (!string.IsNullOrEmpty(filter.User))
        {
            tickets = tickets.Where(x => x.CreatedBy.ToString() == filter.User).ToList();
        }

        if (!string.IsNullOrEmpty(filter.Customer))
        {
            tickets = tickets.Where(x => x.ClientId == filter.Customer).ToList();
        }

        if (!string.IsNullOrEmpty(filter.Email))
        {
            var userDetails = await _userService.GetAllAsync(new[] { filter.Email });
            tickets = tickets.Where(x => userDetails.Data.Select(y => y.Id.ToString()).ToList().Contains(x.ClientId)).ToList();
        }

        if (!string.IsNullOrEmpty(filter.Ip))
        {
            var restricteList = await _repository.GetListAsync<UserRestrictedIp>(x => x.RestrictAccessIPAddress == filter.Ip);
            var user = restricteList.FirstOrDefault();
            tickets = tickets.Where(x => x.ClientId == user.UserId).ToList();
        }

        var support = new SupportStatisticsDto()
        {
            StarDate = filter.StartDate.ToString("yyyy-MM-dd"),
            EndDate = filter.EndDate.ToString("yyyy-MM-dd"),
            Support = new List<ReportSupportDto>(),
        };
        switch (filter.ReportType)
        {
            case ReportType.Normal:
                FillReportAsync("Normal", tickets);
                break;
            case ReportType.ByCustomer:
                func = x => x.LastModifiedBy.ToString() == x.ClientId;
                FillReportAsync("Customer", tickets);
                break;
            case ReportType.ByAgent:
                func = x => x.LastModifiedBy.ToString() == x.AssignedTo;
                FillReportAsync("Agent", tickets);
                break;
            case ReportType.ByEmail:
                func = x => x.CreatedBy.ToString() == x.AssignedTo;
                FillReportAsync("Email", tickets);
                break;
            case ReportType.LinkedIp:
                var restricted = await _repository.GetListAsync<UserRestrictedIp>(null);
                var userList = restricted.Select(x => x.UserId).ToList();
                func = x => userList.Contains(x.CreatedBy.ToString());
                FillReportAsync("LinkedIp", tickets);
                break;
            case ReportType.Status:
                FillReportByStatusAsync("Status", tickets);
                break;
            case ReportType.Department:
                var depts = tickets.Select(y => y.DepartmentId).Distinct().ToList();
                var departments = await _repository.GetListAsync<Department>(x => depts.Contains(x.Id.ToString()));
                FillReportDepartment("Department", tickets, departments);
                break;
            default:
                FillReportAsync("All", tickets);
                break;
        }

        return await Result<SupportStatisticsDto>.SuccessAsync(support);

        void FillReportAsync(string reportName, IEnumerable<Ticket> tickets)
        {
            var tikcetIds = tickets.Select(x => x.Id).ToList();

            var ticketComments = _repository.GetListAsync<TicketComment>(x => tikcetIds.Contains(x.Ticket.Id) && x.CreatedOn >= filter.StartDate && x.CreatedOn < filter.EndDate).Result;

            support.Support.Add(new ReportSupportDto()
            {
                ReportBy = reportName,
                FilterBy = "Hourly",
                SupportReplyDetails = (from ti in ticketComments
                                       group ti by new { Hourly = ti.CreatedOn.ToString("HH") } into grp
                                       select new SupportReplyDetailDto
                                       {
                                           Hourly = grp.Key.Hourly,
                                           Count = grp.Count()
                                       }).ToList(),

            });
            support.Support.Add(new ReportSupportDto()
            {
                ReportBy = reportName,
                FilterBy = "Day",
                SupportReplyDetails = (from ti in ticketComments
                                       group ti by new { Day = ti.CreatedOn.ToString("dd") } into grp
                                       select new SupportReplyDetailDto
                                       {
                                           Day = grp.Key.Day,
                                           Count = grp.Count()
                                       }).ToList(),

            });
            support.Support.Add(new ReportSupportDto()
            {
                ReportBy = reportName,
                FilterBy = "Month",
                SupportReplyDetails = (from ti in ticketComments
                                       group ti by new { Month = ti.CreatedOn.ToString("MMMM") } into grp
                                       select new SupportReplyDetailDto
                                       {
                                           Month = grp.Key.Month,
                                           Count = grp.Count()
                                       }).ToList(),

            });
            support.Support.Add(new ReportSupportDto()
            {
                ReportBy = reportName,
                FilterBy = "Year",
                SupportReplyDetails = (from ti in ticketComments
                                       group ti by new { Year = ti.CreatedOn.ToString("yyyy") } into grp
                                       select new SupportReplyDetailDto
                                       {
                                           Year = grp.Key.Year,
                                           Count = grp.Count()
                                       }).ToList(),

            });
        }

        void FillReportByStatusAsync(string reportName, IEnumerable<Ticket> tickets)
        {
            var spec = new BaseSpecification<TicketComment>();
            spec.Includes.Add(a => a.Ticket);
            var tikcetIds = tickets.Select(x => x.Id).ToList();
            var ticketComments = _repository.GetSearchResultsAsync<TicketComment, TicketCommentDto>(0, int.MaxValue, null, Shared.DTOs.Filters.OrderTypeEnum.OrderByAscending, null, null, x => tikcetIds.Contains(x.Ticket.Id) && x.CreatedOn >= filter.StartDate && x.CreatedOn < filter.EndDate, spec).Result;

            var list3 = (from ti in ticketComments.Data
                         group ti by new { Status = ti.Ticket.TicketStatus, Hourly = ti.CreatedOn.ToString("HH") } into grp
                         select new
                         {
                             Hourly = grp.Key.Hourly,
                             Count = grp.Count(),
                             Status = grp.Key.Status.ToString()
                         }).ToList();
            var statuslist = list3.Select(x => x.Status).Distinct().ToList();
            foreach (string item in statuslist)
            {
                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Hourly",
                    Status = item,
                    SupportReplyDetails = list3.Where(x => x.Status == item).Select(x => new SupportReplyDetailDto()
                    {
                        Hourly = x.Hourly,
                        Count = x.Count,
                    }).ToList()

                });
            }

            var list = (from ti in ticketComments.Data
                        group ti by new { Status = ti.Ticket.TicketStatus, Day = ti.CreatedOn.ToString("dd") } into grp
                        select new
                        {
                            Day = grp.Key.Day,
                            Count = grp.Count(),
                            Status = grp.Key.Status.ToString()
                        }).ToList();
            statuslist = list.Select(x => x.Status).Distinct().ToList();
            foreach (string item in statuslist)
            {
                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Day",
                    Status = item,
                    SupportReplyDetails = list.Where(x => x.Status == item).Select(x => new SupportReplyDetailDto()
                    {
                        Day = x.Day,
                        Count = x.Count,
                    }).ToList()

                });
            }

            var list1 = (from ti in ticketComments.Data
                         group ti by new { Status = ti.Ticket.TicketStatus, Month = ti.CreatedOn.ToString("MMMM") } into grp
                         select new
                         {
                             Month = grp.Key.Month,
                             Count = grp.Count(),
                             Status = grp.Key.Status.ToString()
                         }).ToList();
            statuslist = list1.Select(x => x.Status).Distinct().ToList();
            foreach (string item in statuslist)
            {
                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Month",
                    Status = item,
                    SupportReplyDetails = list1.Where(x => x.Status == item).Select(x => new SupportReplyDetailDto()
                    {
                        Month = x.Month,
                        Count = x.Count,
                    }).ToList()

                });
            }

            var list2 = (from ti in ticketComments.Data
                         group ti by new { Status = ti.Ticket.TicketStatus, Year = ti.CreatedOn.ToString("yyyy") } into grp
                         select new
                         {
                             Year = grp.Key.Year,
                             Count = grp.Count(),
                             Status = grp.Key.Status.ToString()
                         }).ToList();
            statuslist = list2.Select(x => x.Status).Distinct().ToList();
            foreach (string item in statuslist)
            {

                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Year",
                    Status = item,
                    SupportReplyDetails = list2.Where(x => x.Status == item).Select(x => new SupportReplyDetailDto()
                    {
                        Year = x.Year,
                        Count = x.Count,
                    }).ToList()
                });
            }
        }

        void FillReportDepartment(string reportName, IEnumerable<Ticket> tickets, List<Department> departments)
        {
            var spec = new BaseSpecification<TicketComment>();
            spec.Includes.Add(a => a.Ticket);
            var tikcetIds = tickets.Select(x => x.Id).ToList();
            var ticketComments = _repository.GetSearchResultsAsync<TicketComment, TicketCommentDto>(0, int.MaxValue, null, Shared.DTOs.Filters.OrderTypeEnum.OrderByAscending, null, null, x => tikcetIds.Contains(x.Ticket.Id) && x.CreatedOn >= filter.StartDate && x.CreatedOn < filter.EndDate, spec).Result;

            var list = (from ti in ticketComments.Data
                        group ti by new { Department = ti.Ticket.DepartmentId, Hourly = ti.CreatedOn.ToString("HH") } into grp
                        select new
                        {
                            Hourly = grp.Key.Hourly,
                            Count = grp.Count(),
                            Department = departments.FirstOrDefault(x => x.Id.ToString() == grp.Key.Department).Name
                        }).ToList();
            var distictList = list.Select(x => x.Department).Distinct().ToList();
            foreach (string item in distictList)
            {
                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Hourly",
                    Department = item,
                    SupportReplyDetails = (from ti in ticketComments.Data
                                           group ti by new { Department = ti.Ticket.DepartmentId, Hourly = ti.CreatedOn.ToString("HH") } into grp
                                           select new
                                           {
                                               Hourly = grp.Key.Hourly,
                                               Count = grp.Count(),
                                               Department = departments.FirstOrDefault(x => x.Id.ToString() == grp.Key.Department).Name
                                           }).ToList().Where(x => x.Department == item).Select(x => new SupportReplyDetailDto()
                                           {
                                               Hourly = x.Hourly,
                                               Count = x.Count,
                                           }).ToList()

                });
            }

            var list1 = (from ti in ticketComments.Data
                         group ti by new { Department = ti.Ticket.DepartmentId, Day = ti.CreatedOn.ToString("dd") } into grp
                         select new
                         {
                             Day = grp.Key.Day,
                             Count = grp.Count(),
                             Department = departments.FirstOrDefault(x => x.Id.ToString() == grp.Key.Department).Name
                         }).ToList();
            distictList = list.Select(x => x.Department).Distinct().ToList();
            foreach (string item in distictList)
            {
                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Day",
                    Department = item,
                    SupportReplyDetails = list1.Where(x => x.Department == item).Select(x => new SupportReplyDetailDto()
                    {
                        Day = x.Day,
                        Count = x.Count,
                    }).ToList()

                });
            }

            var list2 = (from ti in ticketComments.Data
                         group ti by new { Department = ti.Ticket.DepartmentId, Month = ti.CreatedOn.ToString("MMMM") } into grp
                         select new
                         {
                             Month = grp.Key.Month,
                             Count = grp.Count(),
                             Department = departments.FirstOrDefault(x => x.Id.ToString() == grp.Key.Department).Name
                         }).ToList();
            distictList = list.Select(x => x.Department).Distinct().ToList();
            foreach (string item in distictList)
            {
                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Month",
                    Department = item,
                    SupportReplyDetails = list2.Where(x => x.Department == item).Select(x => new SupportReplyDetailDto()
                    {
                        Month = x.Month,
                        Count = x.Count,
                    }).ToList()

                });
            }

            var list3 = (from ti in ticketComments.Data
                         group ti by new { Department = ti.Ticket.DepartmentId, Year = ti.CreatedOn.ToString("yyyy") } into grp
                         select new
                         {
                             Year = grp.Key.Year,
                             Count = grp.Count(),
                             Department = departments.FirstOrDefault(x => x.Id.ToString() == grp.Key.Department).Name
                         }).ToList();
            distictList = list.Select(x => x.Department).Distinct().ToList();
            foreach (string item in distictList)
            {
                support.Support.Add(new ReportSupportDto()
                {
                    ReportBy = reportName,
                    FilterBy = "Year",
                    Department = item,
                    SupportReplyDetails = list3.Where(x => x.Department == item).Select(x => new SupportReplyDetailDto()
                    {
                        Year = x.Year,
                        Count = x.Count,
                    }).ToList()

                });
            }
        }
    }

}
