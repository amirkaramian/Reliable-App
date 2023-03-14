using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Incomes;
public class IncomeHistoryDto
{
    public List<IncomeOverallDto> IncomeOveralls { get; set; }
}

public class IncomeOverallDto
{
    public string Year { get; set; }
    public decimal Total { get; set; }
    public List<IncomeHistoryMontlyDto> MontlyIncomes { get; set; }
}

public class IncomeHistoryMontlyDto
{
    public string Month { get; set; }
    public decimal Income { get; set; }
}
