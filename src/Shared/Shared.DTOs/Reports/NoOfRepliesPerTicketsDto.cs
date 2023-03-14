using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Reports;

public class NoOfRepliesPerTicketsDto : IDto
{
    public IEnumerable<NoOfRepliesPerTicketsDetailsDto> NoOfRepliesPerTicketsDetails { get; set; }
}
