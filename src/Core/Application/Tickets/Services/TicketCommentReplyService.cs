using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Tickets.Interfaces;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Tickets;
using MyReliableSite.Domain.Dashboard;
using MyReliableSite.Shared.DTOs.Tickets;
using MyReliableSite.Application.Specifications;
using Mapster;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Shared.DTOs.Notifications;
using MyReliableSite.Shared.DTOs.Notifications.Enums;
using MyReliableSite.Domain.Tickets.Events;

namespace MyReliableSite.Application.Tickets.Services;

public class TicketCommentReplyService : ITicketCommentReplyService
{
    private readonly IStringLocalizer<TicketCommentReplyService> _localizer;
    private readonly IRepositoryAsync _repository;

    private readonly ICurrentUser _user;
    private readonly INotificationService _notificationService;

    public TicketCommentReplyService()
    {
    }

    public TicketCommentReplyService(IRepositoryAsync repository, ICurrentUser user, IStringLocalizer<TicketCommentReplyService> localizer, INotificationService notificationService)
    {
        _repository = repository;
        _user = user;
        _localizer = localizer;
        _notificationService = notificationService;
    }

    public async Task<Result<Guid>> CreateTicketCommentReplyAsync(CreateTicketCommentReplyRequest request)
    {
        var spec = new BaseSpecification<TicketComment>();
        spec.Includes.Add(x => x.Ticket);

        var ticketComment = await _repository.GetByIdAsync<TicketComment>(request.TicketCommentId, spec);
        if (ticketComment == null) throw new EntityNotFoundException(string.Format(_localizer["TicketComment.notfound"], request.TicketCommentId));
        if (request.TicketCommentParentReplyId != null)
        {
            var ticketCommentReplyCheck = await _repository.GetByIdAsync<TicketCommentReply>(request.TicketCommentParentReplyId.Value);
            if (ticketCommentReplyCheck == null) throw new EntityNotFoundException(string.Format(_localizer["TicketCommentReply.notfound"], request.TicketCommentParentReplyId));
        }

        string userId = _user.GetUserId().ToString();
        var ticketCommentReply = new TicketCommentReply(request.CommentText, userId, request.TicketCommentParentReplyId);
        ticketCommentReply.TicketComment = ticketComment;
        ticketCommentReply.DomainEvents.Add(new TicketCommentReplyCreatedEvent(ticketCommentReply));
        var ticketCommentReplyId = await _repository.CreateAsync<TicketCommentReply>((TicketCommentReply)ticketCommentReply);

        await _repository.SaveChangesAsync();
        var ticket = await _repository.GetByIdAsync<Ticket>(ticketComment.Ticket.Id);
        await _repository.ClearCacheAsync<Ticket>(ticket);

        if (_user.IsInRole("Client"))
        {
            string message = $"Hello [[fullName]], you received a new reply on the ticket.";
            await _notificationService.SendMessageToUserAsync(ticketComment.Ticket.AssignedTo, new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.TICKET_NEW_REPLY, TargetUserTypes = NotificationTargetUserTypes.Admins, NotifyModelId = ticket.Id, Data = new { TicketId = ticket.Id, TicketCommentReplyId = ticketCommentReplyId, TicketCommentId = request.TicketCommentId, TicketCommentParentReplyId = request.TicketCommentParentReplyId } });
        }

        await createTicketHistoryAsync(ticket, ticketComment.Id, ticketCommentReply);
        return await Result<Guid>.SuccessAsync(ticketCommentReplyId);
    }

    public async Task<Result<Guid>> DeleteTicketCommentReplyAsync(Guid id)
    {

        var spec = new BaseSpecification<TicketCommentReply>();
        spec.Includes.Add(a => a.TicketComment.Ticket);
        var ticketCommentReply = await _repository.GetByIdAsync<TicketCommentReply>(id, spec);
        if (ticketCommentReply == null) throw new EntityNotFoundException(string.Format(_localizer["TicketCommentReply.notfound"], id));
        var ticketToDelete = await _repository.RemoveByIdAsync<TicketCommentReply>(id);
        ticketToDelete.DomainEvents.Add(new TicketCommentReplyDeletedEvent(ticketToDelete));

        await _repository.SaveChangesAsync();
        var ticket = await _repository.GetByIdAsync<Ticket>(ticketToDelete.TicketComment.Ticket.Id);
        await _repository.ClearCacheAsync<Ticket>(ticket);

        // user assignment to default ticket
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> UpdateTicketCommentReplyAsync(UpdateTicketCommentReplyRequest request, Guid id)
    {

        var spec = new BaseSpecification<TicketCommentReply>();
        spec.Includes.Add(a => a.TicketComment.Ticket);
        var ticketCommentReply = await _repository.GetByIdAsync<TicketCommentReply>(id, spec);
        if (ticketCommentReply == null) throw new EntityNotFoundException(string.Format(_localizer["TicketCommentReply.notfound"], id));
        var updatedTicketCommentReply = ticketCommentReply.Update(request.CommentText, request.TicketCommentParentReplyId);
        updatedTicketCommentReply.DomainEvents.Add(new TicketCommentReplyUpdatedEvent(updatedTicketCommentReply));
        await _repository.UpdateAsync<TicketCommentReply>(updatedTicketCommentReply);

        await _repository.SaveChangesAsync();
        var ticket = await _repository.GetByIdAsync<Ticket>(ticketCommentReply.TicketComment.Ticket.Id);
        await _repository.ClearCacheAsync<Ticket>(ticket);

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<TicketCommentReplyDto>> GetTicketCommentReplyAsync(Guid id)
    {
        var spec = new BaseSpecification<TicketCommentReply>();
        spec.Includes.Add(a => a.TicketCommentChildReplies);
        var ticket = await _repository.GetByIdAsync<TicketCommentReply, TicketCommentReplyDto>(id, spec);
        return await Result<TicketCommentReplyDto>.SuccessAsync(ticket);
    }

    private async Task createTicketHistoryAsync(Ticket request, Guid ticketCommentId, TicketCommentReply ticketCommentReply)
    {
        var ticketCommentReplyHistory = new TicketCommentReplyHistory(ticketCommentReply.CommentText, ticketCommentReply.UserId, ticketCommentReply.TicketCommentParentReplyId);
        ticketCommentReplyHistory.TicketCommentId = ticketCommentId;

        _ = await _repository.CreateAsync((TicketCommentReplyHistory)ticketCommentReplyHistory);

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
            ticketCommentReplyHistory.TicketCommentId,
            ticketCommentReply.Id,
            null,
            ticketCommentReplyHistory.Id,
            request.IncomingFromClient);

        await _repository.CreateAsync<TicketHistory>((TicketHistory)ticketHistory);
        await _repository.SaveChangesAsync();

    }

}
