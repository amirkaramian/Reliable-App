using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Tickets;
public class SupportHistoryFilterDto
{
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
    public ReportType ReportType { get; set; }
    public string Ip { get; set; }
    public string Customer { get; set; }
    public string User { get; set; }
    public string Email { get; set; }
}

public enum ReportType
{
    Normal,
    ByCustomer,
    ByAgent,
    ByEmail,
    LinkedIp,
    Status,
    Department
}