using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Reports;

public class AnnualIncomeDetailsDto : IDto
{
    public DateTime Month { get; set; }
    public decimal TotalAmount { get; set; }
}
