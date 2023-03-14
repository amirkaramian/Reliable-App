using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Reports.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Tickets;
using MyReliableSite.Shared.DTOs.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Application.Reports.Services;

public class ReportService : IReportService
{
    private readonly IStringLocalizer<ReportService> _localizer;
    private readonly IRepositoryAsync _repository;

    public ReportService()
    {
    }

    public ReportService(IRepositoryAsync repository, IStringLocalizer<ReportService> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    // Annual Income Report Per year by month with Last 3 year calulcation Overall income (excluding current year)
    public async Task<Result<AnnualIncomeReportDto>> AnnualIncomeReport(int year)
    {
        AnnualIncomeReportDto data = new AnnualIncomeReportDto();

        data.annualIncomeDetails = await _repository.QueryWithDtoAsync<AnnualIncomeDetailsDto>($@"declare @Year varchar(100) = YEAR('{year}');
                                                                                            SELECT CONVERT(NVARCHAR(7), t.CreatedOn, 121) AS [Month], ISNULL(SUM(t.Total), 0) AS [TotalAmount]
                                                                                            FROM Transactions t
                                                                                            INNER JOIN Orders o ON t.ReferenceId = o.Id AND t.TransactionStatus = 1 AND t.TransactionType = 0
                                                                                            WHERE YEAR(t.CreatedOn) = @Year
                                                                                            GROUP BY CONVERT(NVARCHAR(7), t.CreatedOn, 121) ORDER BY [Month]");

        var totalAmount = await _repository.QueryWithDtoAsync<dynamic>($@"SELECT ISNULL(SUM(t.Total), 0) AS [TotalAmount]
                                                                                            FROM Transactions t
                                                                                            INNER JOIN Orders o ON t.ReferenceId = o.Id AND t.TransactionStatus = 1 AND t.TransactionType = 0
                                                                                            WHERE (YEAR(t.CreatedOn) >= year(getdate()) - 3) AND (YEAR(t.CreatedOn) <= year(getdate()) - 1)");

        if(totalAmount != null && totalAmount.Count() > 0)
            data.TotalAmount = Convert.ToDecimal(totalAmount.FirstOrDefault().TotalAmount);

        return await Result<AnnualIncomeReportDto>.SuccessAsync(data);
    }

    // Tickets related fields
    public async Task<Result<IEnumerable<DateAndCountDto>>> TicketReportFields(ReportFields field, DateTime startDate, DateTime endDate)
    {
        if (field == ReportFields.ByCustomer)
            return await TicketsByCustomer(startDate, endDate);
        else if (field == ReportFields.ByAgent)
            return await TicketsByAgents(startDate, endDate);
        else if (field == ReportFields.ByStatus)
            return await TicketsByStatus(startDate, endDate);
        else if (field == ReportFields.ByDepartment)
            return await TicketsByDepartments(startDate, endDate);
        else if (field == ReportFields.ByPriority)
            return await TicketsByPriority(startDate, endDate);

        return await Result<IEnumerable<DateAndCountDto>>.SuccessAsync(new List<DateAndCountDto>());
    }

    // Numbers of Replies per tickets
    public async Task<Result<NoOfRepliesPerTicketsDto>> NoOfRepliesPerTickets(DateTime startDate, DateTime endDate)
    {
        NoOfRepliesPerTicketsDto data = new NoOfRepliesPerTicketsDto();

        data.NoOfRepliesPerTicketsDetails = await _repository.QueryWithDtoAsync<NoOfRepliesPerTicketsDetailsDto>($@"drop table if exists #CommentRepies
                                                                                                                SELECT t.Id TicketId,tc.Id CommentId,CONVERT(NVARCHAR(10),t.createdOn,121) as CreatedOn,
                                                                                                                SUM (case WHEN tcr.Id IS NOT NULL THEN 1 ELSE 0 END) AS TicketRepliesCount
                                                                                                                into #CommentRepies
                                                                                                                from Tickets t (nolock)
                                                                                                                Left JOIN TicketComments tc (nolock) On t.Id = tc.TicketId
                                                                                                                LEFT JOIN  TicketCommentReplies tcr(nolock)  ON tc.Id = tcr.TicketCommentId
                                                                                                                LEFT JOIN TicketCommentReplies tcrr (nolock) on tcr.Id =tcrr.TicketCommentParentReplyId
                                                                                                                GROUP BY t.Id,tc.Id, CONVERT(NVARCHAR(10),t.createdOn,121)
                                                                                                                SELECT SUM(cr.TicketRepliesCount) as TicketReplies,cr.TicketId, cr.CreatedOn FROM #CommentRepies cr
                                                                                                                WHERE ((CONVERT(date, [cr].[CreatedOn]) >= '{startDate}') AND (CONVERT(date, [cr].[CreatedOn]) <= '{endDate}'))
                                                                                                                GROUP By cr.TicketId, cr.CreatedOn");

        return await Result<NoOfRepliesPerTicketsDto>.SuccessAsync(data);
    }

    // Ticket Response Time in Seconds
    public async Task<Result<TicketResponseTimeDto>> TicketsResponseTime(DateTime startDate, DateTime endDate)
    {
        TicketResponseTimeDto data = new TicketResponseTimeDto();

        var responseTimeRecords = await _repository.QueryWithDtoAsync<TicketResponseTimeDetailsDto>($@"SELECT ISNULL(DATEDIFF(ss, t.CreatedOn, tc.CreatedOn), 0) AS [Response], t.Id, t.CreatedOn 
                                                                                                                FROM Tickets t
                                                                                                                INNER JOIN TicketComments tc ON t.Id = tc.TicketId
                                                                                                                WHERE ((CONVERT(date, [t].[CreatedOn]) >= '{startDate}') AND (CONVERT(date, [t].[CreatedOn]) <= '{endDate}'))
                                                                                                                ORDER BY tc.CreatedOn ASC");
        var filteration = new List<TicketResponseTimeDetailsDto>();

        if (responseTimeRecords != null && responseTimeRecords.Count() > 0)
        {
            foreach(var item in responseTimeRecords)
            {
                if(filteration.FirstOrDefault(x => x.Id == item.Id) == null)
                    filteration.Add(item);
            }
        }

        data.TicketResponseTimeDetails = filteration;

        return await Result<TicketResponseTimeDto>.SuccessAsync(data);
    }

    private async Task<Result<IEnumerable<DateAndCountDto>>> TicketsByCustomer(DateTime startDate, DateTime endDate)
    {
        var tickets = await _repository.GetListAsync<Ticket>(x => x.CreatedOn.Date >= startDate.Date && x.CreatedOn.Date <= endDate.Date);
        var data = tickets.GroupBy(group => new { group.CreatedBy, group.CreatedOn.Date }).Select(x => new DateAndCountDto() { CreatedOn = x.Key.Date, Count = x.Count() });

        return await Result<IEnumerable<DateAndCountDto>>.SuccessAsync(data);
    }

    private async Task<Result<IEnumerable<DateAndCountDto>>> TicketsByAgents(DateTime startDate, DateTime endDate)
    {
        var tickets = await _repository.GetListAsync<Ticket>(x => x.CreatedOn.Date >= startDate.Date && x.CreatedOn.Date <= endDate.Date);
        var data = tickets.GroupBy(group => new { group.AssignedTo, group.CreatedOn.Date }).Select(x => new DateAndCountDto() { CreatedOn = x.Key.Date, Count = x.Count() });

        return await Result<IEnumerable<DateAndCountDto>>.SuccessAsync(data);
    }

    private async Task<Result<IEnumerable<DateAndCountDto>>> TicketsByStatus(DateTime startDate, DateTime endDate)
    {
        var tickets = await _repository.GetListAsync<Ticket>(x => x.CreatedOn.Date >= startDate.Date && x.CreatedOn.Date <= endDate.Date);
        var data = tickets.GroupBy(group => new { group.TicketStatus, group.CreatedOn.Date }).Select(x => new DateAndCountDto() { CreatedOn = x.Key.Date, Count = x.Count() });

        return await Result<IEnumerable<DateAndCountDto>>.SuccessAsync(data);
    }

    private async Task<Result<IEnumerable<DateAndCountDto>>> TicketsByDepartments(DateTime startDate, DateTime endDate)
    {
        var tickets = await _repository.GetListAsync<Ticket>(x => x.CreatedOn.Date >= startDate.Date && x.CreatedOn.Date <= endDate.Date);
        var data = tickets.GroupBy(group => new { group.DepartmentId, group.CreatedOn.Date }).Select(x => new DateAndCountDto() { CreatedOn = x.Key.Date, Count = x.Count() });

        return await Result<IEnumerable<DateAndCountDto>>.SuccessAsync(data);
    }

    private async Task<Result<IEnumerable<DateAndCountDto>>> TicketsByPriority(DateTime startDate, DateTime endDate)
    {
        var tickets = await _repository.GetListAsync<Ticket>(x => x.CreatedOn.Date >= startDate.Date && x.CreatedOn.Date <= endDate.Date);
        var data = tickets.GroupBy(group => new { group.TicketPriority, group.CreatedOn.Date }).Select(x => new DateAndCountDto() { CreatedOn = x.Key.Date, Count = x.Count() });

        return await Result<IEnumerable<DateAndCountDto>>.SuccessAsync(data);
    }
}
