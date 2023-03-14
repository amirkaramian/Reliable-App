using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Orders;
public class OrderDto : IDto
{
    public Guid Id { get; set; }
    public int OrderNo { get; set; }
    public string BillNo { get; set; }
    public string ClientId { get; set; }
    public List<string> AdminAssigned { get; set; }

    public DateTime CreatedOn { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public string CustomerIP { get; set; }
    public string Notes { get; set; }
    public OrderStatus Status { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TotalPrice { get; set; }
    public string ClientFullName { get; set; }
    public string AdminAssignedFullName { get; set; }
    public List<AdminUserInfo> AdminUserInfos { get; set; }
}

public class AdminUserInfo
{
    public string AdminAssigned { get; set; }
    public string AdminAssignedFullName { get; set; }
}