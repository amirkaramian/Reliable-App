using MyReliableSite.Shared.DTOs.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Dashboard;
public class DashboardDto : IDto
{
    public IEnumerable<DashboardKeyValueDto> AllOrders { get; set; } // Key is CreatedDate and Value is total no of order
    public IEnumerable<DashboardKeyValueDto> CompletedOrders { get; set; } // Key is CreatedDate and Value is total no of order
    public IEnumerable<DashboardKeyValueDto> Incomes { get; set; } // Key is CreatedDate and Value is sum of Incomes total price of orders
    public IEnumerable<DashboardKeyValueDto> Refunds { get; set; } // Key is CreatedDate and Value is sum of Refunds after retian amount
    public IEnumerable<DashboardKeyValueDto> TicketsDepartment { get; set; } // Key is DepartmentName, Value is total no of tickets against this department
    public IEnumerable<DashboardKeyValueDto> IncomeForecast { get; set; } // Key is Type like Month, Quarter etc, Value is total sum of order total in transaction table
}

public class DataCountDto : IDto
{
    public Dictionary<string, int> Orders { get; set; }
    public Dictionary<string, object> Tickets { get; set; }
    public int BillCount { get; set; }
    public int AdminGroupCount { get; set; }
    public int TransactionsCount { get; set; }
    public int ArticlesCount { get; set; }
    public int WebHooksCount { get; set; }
    public int AdminsCount { get; set; }
    public int ClientsCount { get; set; }
    public int SuperAdminCounts { get; set; }
    public int ProductsCount { get; set; }
    public Dictionary<string, int> Products { get; set; }
    public Dictionary<string, int> KnowledgeBase { get; set; }
}