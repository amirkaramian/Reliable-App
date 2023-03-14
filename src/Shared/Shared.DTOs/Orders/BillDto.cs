using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Orders;
public class BillDto : IDto
{
    public Guid Id { get; set; }
    public string BillNo { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public OrderStatus Status { get; set; }
    public Guid OrderId { get; set; }
    public DateTime DueDate { get; set; }
    public Guid ProductId { get; set; }
    public OrderDetailsDto Order { get; set; }
    public OrderedProductDetailDto Product { get; set; }
    public string UserImagePath { get; set; }
    public string FullName { get; set; }
    public string IssuedBy { get; set; } // will get brand name if any brand assigned to the client and if no brand is assigned then i will get company name from settings.
    public string IssueByImage { get; set; }
    public string IssuedFor { get; set; } // get company name from user detail if no company name found i will get full name of client.
    public string IssueForImage { get; set; }
    public string Tenant { get; set; }
}
