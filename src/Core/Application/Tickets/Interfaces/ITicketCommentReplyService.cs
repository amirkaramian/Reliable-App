using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.Tickets;

namespace MyReliableSite.Application.Tickets.Interfaces;

public interface ITicketCommentReplyService : ITransientService
{
    Task<Result<Guid>> CreateTicketCommentReplyAsync(CreateTicketCommentReplyRequest request);

    Task<Result<Guid>> UpdateTicketCommentReplyAsync(UpdateTicketCommentReplyRequest request, Guid id);

    Task<Result<Guid>> DeleteTicketCommentReplyAsync(Guid id);
    Task<Result<TicketCommentReplyDto>> GetTicketCommentReplyAsync(Guid id);
}
