using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Reports;

public class TicketResponseTimeDto : IDto
{
    public IEnumerable<TicketResponseTimeDetailsDto> TicketResponseTimeDetails { get; set; }
}
