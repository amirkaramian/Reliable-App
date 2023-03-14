using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.Tickets;

namespace MyReliableSite.Application.Tickets.Interfaces;

public interface ITicketCommentService : ITransientService
{
    Task<PaginatedResult<TicketCommentDto>> SearchAsync(TicketCommentListFilter filter);

    Task<Result<Guid>> CreateTicketCommentAsync(CreateTicketCommentRequest request);

    Task<Result<Guid>> UpdateTicketCommentAsync(UpdateTicketCommentRequest request, Guid id);

    Task<Result<Guid>> DeleteTicketCommentAsync(Guid id);
    Task<Result<Guid>> DeleteClientCommentReplyAsync(Guid id);
    Task<Result<TicketCommentDto>> GetTicketCommentAsync(Guid id);
}
