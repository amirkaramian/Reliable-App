using MyReliableSite.Shared.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Orders;
public class OrderDetailsDto : IDto
{
    public int OrderNo { get; set; }
    public string CustomerIP { get; set; }
    public string ClientId { get; set; }
    public string AdminAssigned { get; set; }
    public string ClientFullName { get; set; }
    public string AdminAssignedFullName { get; set; }

    public string Notes { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public OrderStatus Status { get; set; }
    public OrderBillDto Bill { get; set; }
    public ICollection<ProductDetailDto> Products { get; set; }
    public ICollection<ProductLineItemDto> OrderProductLineItems { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TotalPrice { get; set; }
    public List<AdminUserInfo> AdminUserInfos { get; set; }
}