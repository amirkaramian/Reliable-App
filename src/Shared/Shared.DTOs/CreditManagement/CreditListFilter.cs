using MyReliableSite.Shared.DTOs.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.CreditManagement;
public class CreditListFilter : PaginationFilter
{
    public bool IsDeleted { get; set; }
    public string ClientId { get; set; }
}
