using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Dashboard;
public class ClientDashboardDto
{
    public List<TicketItemDto> TicketItems { get; set; }
    public TicketStatistics TicketStatistics { get; set; }
    public ProductStatistics ProductServiceStatistics { get; set; }
    public InvoiceStatistics InvoiceStatistics { get; set; }
}

public class TicketItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string DepartmentName { get; set; }
    public string AssignedTo { get; set; }
    public string IdleTime { get; set; }
    public string Status { get; set; }
    public int MessageCount { get; set; }
}

public class TicketStatistics
{
    public int OpenTotal { get; set; }
    public int WatingToAgentCount { get; set; }
    public int WatingToClientCount { get; set; }
}

public class ProductStatistics
{
    public int Total { get; set; }
    public int ActiveCount { get; set; }
    public int PendingCount { get; set; }
    public int SuspendedCount { get; set; }
}

public class InvoiceStatistics
{
    public int UnPaidCount { get; set; }
    public int OverdueCount { get; set; }
}