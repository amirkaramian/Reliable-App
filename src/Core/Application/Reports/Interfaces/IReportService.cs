using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Application.Reports.Interfaces;

public interface IReportService : ITransientService
{
    Task<Result<AnnualIncomeReportDto>> AnnualIncomeReport(int year);
    Task<Result<IEnumerable<DateAndCountDto>>> TicketReportFields(ReportFields field, DateTime startDate, DateTime endDate);
    Task<Result<NoOfRepliesPerTicketsDto>> NoOfRepliesPerTickets(DateTime startDate, DateTime endDate);
    Task<Result<TicketResponseTimeDto>> TicketsResponseTime(DateTime startDate, DateTime endDate);
}
