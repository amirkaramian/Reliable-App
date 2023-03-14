using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Reports;

public class NoOfRepliesPerTicketsDetailsDto : IDto
{
    public int TicketReplies { get; set; }
    public Guid TicketId { get; set; }
    public DateTime CreatedOn { get; set; }
}
