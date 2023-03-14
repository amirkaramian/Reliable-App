using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Orders;
public class OrderBillDto : IDto
{
    public Guid Id { get; set; }
    public string BillNo { get; set; }
    public string UserId { get; set; }
    public OrderedProductDetailDto Product { get; set; }
    public OrderStatus Status { get; set; }
    public Guid OrderId { get; set; }
    public DateTime DueDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal VAT { get; set; }
    public List<OrderProductLineItemsDto> OrderProductLineItems { get; set; }
}