using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Reports;

public class AnnualIncomeReportDto : IDto
{
    public IEnumerable<AnnualIncomeDetailsDto> annualIncomeDetails { get; set; } = new List<AnnualIncomeDetailsDto>();
    public decimal TotalAmount { get; set; } = 0;
}