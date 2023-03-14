using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Tickets.Interfaces;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Tickets;
using MyReliableSite.Shared.DTOs.Tickets;
using MyReliableSite.Application.Specifications;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Shared.DTOs.Notifications;
using MyReliableSite.Shared.DTOs.Notifications.Enums;
using MyReliableSite.Domain.Tickets.Events;
using MyReliableSite.Application.Storage;

namespace MyReliableSite.Application.Tickets.Services;

public class TicketCommentService : ITicketCommentService
{
    private readonly IStringLocalizer<TicketCommentService> _localizer;
    private readonly IRepositoryAsync _repository;
    private readonly IUserService _userService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUser _user;
    private readonly INotificationService _notificationService;
    public TicketCommentService()
    {
    }

    public TicketCommentService(IRepositoryAsync repository, IUserService userService, IFileStorageService fileStorageService, ICurrentUser user, IStringLocalizer<TicketCommentService> localizer, INotificationService notificationService)
    {
        _repository = repository;
        _userService = userService;
        _fileStorageService = fileStorageService;
        _user = user;
        _localizer = localizer;
        _notificationService = notificationService;
    }

    public async Task<Result<Guid>> CreateTicketCommentAsync(CreateTicketCommentRequest request)
    {
        var ticket = await _repository.GetByIdAsync<Ticket>(request.TicketId);

        if (ticket == null) throw new EntityNotFoundException(string.Format(_localizer["Ticket.notfound"], request.TicketId));
        string userId = _user.GetUserId().ToString();
        var ticketComment = new TicketComment(request.CommentText, userId, request.IsSticky, request.TicketCommentAction, request.TicketCommentType, request.IsDraft);
        ticketComment.Ticket = ticket;
        ticketComment.DomainEvents.Add(new TicketCommentCreatedEvent(ticketComment));
        var ticketcommentId = await _repository.CreateAsync<TicketComment>((TicketComment)ticketComment);

        await _repository.SaveChangesAsync();

        await _repository.ClearCacheAsync<Ticket>(ticket);

        if (_user.IsInRole("Client"))
        {
            string message = $"Hello [[fullName]], you received a new comment to ticket.";
            await _notificationService.SendMessageToUserAsync(ticket.AssignedTo, new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.TICKET_NEW_COMMENTS, TargetUserTypes = NotificationTargetUserTypes.Admins, NotifyModelId = ticket.Id, Data = new { TicketId = ticket.Id, TicketCommentId = ticketcommentId } });
        }

        await createTicketHistoryAsync(ticket, ticketComment);

        return await Result<Guid>.SuccessAsync(ticketcommentId);
    }

    public async Task<Result<Guid>> DeleteTicketCommentAsync(Guid id)
    {
        var spec = new BaseSpecification<TicketComment>();
        spec.Includes.Add(a => a.Ticket);
        var ticketComment = await _repository.GetByIdAsync<TicketComment>(id, spec);
        if (ticketComment == null) throw new EntityNotFoundException(string.Format(_localizer["Ticket.notfound"], id));
        var ticketToDelete = await _repository.RemoveByIdAsync<TicketComment>(id);
        ticketToDelete.DomainEvents.Add(new TicketCommentDeletedEvent(ticketComment));

        await _repository.SaveChangesAsync();
        var ticket = await _repository.GetByIdAsync<Ticket>(ticketToDelete.Ticket.Id);
        await _repository.ClearCacheAsync<Ticket>(ticket);

        // user assignment to default ticketComment
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> DeleteClientCommentReplyAsync(Guid id)
    {
        var ticketCommentReply = await _repository.GetByIdAsync<TicketCommentReply>(id);
        if (ticketCommentReply == null) throw new EntityNotFoundException(string.Format(_localizer["ticketCommentReply.notfound"], id));
        var ticketCommentChildReplies = await _repository.GetListAsync<TicketCommentReply>(tcr => tcr.TicketCommentParentReplyId == ticketCommentReply.Id);
        foreach (var item in ticketCommentChildReplies)
        {
            var tcr = await _repository.RemoveByIdAsync<TicketCommentReply>(item.Id);
            tcr.DomainEvents.Add(new TicketCommentReplyDeletedEvent(tcr));
            await _repository.SaveChangesAsync();

            var ticketCommentChild = await _repository.GetByIdAsync<TicketComment>(item.TicketCommentId);
            if (ticketCommentChild != null)
            {
                var ticket = await _repository.GetByIdAsync<Ticket>(ticketCommentChild.TicketId);
                await _repository.ClearCacheAsync<Ticket>(ticket);
            }
        }

        var ticketCommentReplyToDelete = await _repository.RemoveByIdAsync<TicketCommentReply>(id);
        ticketCommentReplyToDelete.DomainEvents.Add(new TicketCommentReplyDeletedEvent(ticketCommentReplyToDelete));

        await _repository.SaveChangesAsync();

        var ticketComment = await _repository.GetByIdAsync<TicketComment>(ticketCommentReplyToDelete.TicketCommentId);
        if (ticketComment != null)
        {
            var ticket = await _repository.GetByIdAsync<Ticket>(ticketComment.TicketId);
            await _repository.ClearCacheAsync<Ticket>(ticket);
        }

        // user assignment to default ticketComment
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<PaginatedResult<TicketCommentDto>> SearchAsync(TicketCommentListFilter filter)
    {
        return await _repository.GetSearchResultsAsync<TicketComment, TicketCommentDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword, m => m.TicketCommentType == filter.TicketCommentType);
    }

    public async Task<Result<Guid>> UpdateTicketCommentAsync(UpdateTicketCommentRequest request, Guid id)
    {

        var spec = new BaseSpecification<TicketComment>();
        spec.Includes.Add(a => a.Ticket);
        var ticketComment = await _repository.GetByIdAsync<TicketComment>(id, spec);
        if (ticketComment == null) throw new EntityNotFoundException(string.Format(_localizer["Ticket.notfound"], id));
        var updatedTicket = ticketComment.Update(request.CommentText, request.IsSticky, request.TicketCommentAction, request.TicketCommentType, request.IsDraft);
        updatedTicket.DomainEvents.Add(new TicketCommentUpdatedEvent(ticketComment));
        await _repository.UpdateAsync<TicketComment>(updatedTicket);

        await _repository.SaveChangesAsync();
        var ticket = await _repository.GetByIdAsync<Ticket>(ticketComment.Ticket.Id);
        await _repository.ClearCacheAsync<Ticket>(ticket);

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<TicketCommentDto>> GetTicketCommentAsync(Guid id)
    {
        var spec = new BaseSpecification<TicketComment>();
        spec.Includes.Add(a => a.TicketCommentReplies);
        var ticketComment = await _repository.GetByIdAsync<TicketComment, TicketCommentDto>(id, spec);

        var userIds = ticketComment.TicketCommentReplies.Select(c => c.UserId).ToList();

        var userDetails = await _userService.GetAllAsync(userIds);

        var assignedToUserObj = userDetails.Data.FirstOrDefault(m => m.Id == Guid.Parse(ticketComment.UserId));
        if (assignedToUserObj != null)
        {
            ticketComment.UserFullName = assignedToUserObj.FullName;
            if (!string.IsNullOrEmpty(assignedToUserObj.ImageUrl))
            {
                ticketComment.UserImagePath = await _fileStorageService.ReturnBase64StringOfImageFileAsync(assignedToUserObj.ImageUrl);
            }
        }

        foreach (var itemReply in ticketComment.TicketCommentReplies)
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

        return await Result<TicketCommentDto>.SuccessAsync(ticketComment);
    }

    private async Task createTicketHistoryAsync(Ticket request, TicketComment ticketComment)
    {
        var ticketCommentHistory = new TicketCommentHistory(ticketComment.CommentText, ticketComment.UserId, ticketComment.IsSticky, ticketComment.TicketCommentAction, ticketComment.TicketCommentType, ticketComment.IsDraft);
        ticketCommentHistory.TicketId = ticketComment.Ticket.Id;

        var ticketCommentReplyId = await _repository.CreateAsync((TicketCommentHistory)ticketCommentHistory);

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
            ticketComment.Id,
            null,
            ticketCommentHistory.Id,
            null,
            request.IncomingFromClient);

        await _repository.CreateAsync<TicketHistory>((TicketHistory)ticketHistory);
        await _repository.SaveChangesAsync();

    }

}
