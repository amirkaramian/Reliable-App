using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Reports;

public class TicketResponseTimeDetailsDto : IDto
{
    public decimal Response { get; set; }
    public Guid Id { get; set; }
    public DateTime CreatedOn { get; set; }
}
