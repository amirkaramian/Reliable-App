using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.Tickets;
using MyReliableSite.Shared.DTOs.Transaction;

namespace MyReliableSite.Application.Tickets.Interfaces;

public interface ITicketService : ITransientService
{
    Task<PaginatedResult<TicketDto>> SearchAsync(TicketListFilter filter);

    Task<Result<Guid>> CreateTicketAsync(CreateTicketRequest request);

    Task<Result<Guid>> UpdateTicketAsync(UpdateTicketRequest request, Guid id);

    Task<Result<Guid>> DeleteTicketAsync(Guid id);
    Task<Result<TicketDto>> GetTicketAsync(Guid id, string clientId = "");
    Task<Result<TicketDto>> GetTicketAsyncWithCommentAndReplies(Guid id);
    Task<Result<TicketDto>> GetClientTicketAsync(Guid id);
    Task<Result<List<TicketDto>>> GetTicketByClientIdAsync(Guid ClientId);
    Task<Result<List<TicketDto>>> GetCurrentUserTicketsAsync();
    Task<Result<List<TicketHistoryDto>>> GetTicketHistoryAsync(Guid id);
    Task<Result<Guid>> CloseTicketAsync(Guid id);
    Task<Result<List<TicketEXL>>> GetTicketListAsync(string userId, DateTime startDate, DateTime endDate);
    Task<Result<SupportStatisticsDto>> GetSupportDurationAsync(SupportHistoryFilterDto filterDto);
    Task<Result<SupportStatisticsDto>> GetResponseTimeTicketAsync(SupportHistoryFilterDto filter);
    Task<Result<SupportStatisticsDto>> GetReplyTicketAsync(SupportHistoryFilterDto filter);

}
