using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Reports;

public class DateAndCountDto : IDto
{
    public DateTime CreatedOn { get; set; }
    public int Count { get; set; }
}

public enum ReportFields
{
    ByCustomer,
    ByAgent,
    ByStatus,
    ByDepartment,
    ByPriority
}