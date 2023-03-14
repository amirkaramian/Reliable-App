using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Products;

public class ProductCounts
{
    public int Active { get; set; }
    public int Suspended { get; set; }
    public int Pending { get; set; }
    public int Cancelled { get; set; }

    public ProductCounts(int active, int suspended, int pending, int cancelled)
    {
        Active = active;
        Suspended = suspended;
        Pending = pending;
        Cancelled = cancelled;
    }

}
