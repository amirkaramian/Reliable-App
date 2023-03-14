using MyReliableSite.Shared.DTOs.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Notifications;

public class NotificationsListFilter : PaginationFilter
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
