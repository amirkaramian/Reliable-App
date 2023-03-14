using MyReliableSite.Shared.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Orders;
public class CreateOrderRequest : IMustBeValid
{
    public List<CreateProductRequest> Products { get; set; }
    public List<string> AdminAssigned { get; set; }
    public string OrderForClientId { get; set; }
    public string CustomerIP { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public string Notes { get; set; }
    public string Tenant { get; set; }
    public bool Notify { get; set; } = false;
}

public enum OrderStatus
{
    Draft = 0, // The order is only visible on the admin interface.
    Pending = 1, // The order is visible on the end user interface.
    Paid = 2, // The invoice has been paid (if applicable).
    Processing = 3, // - The order is being processed.
    Accepted = 4, // The order has been reviewed and accepted.
    Completed = 5, // The order has been setup.
    Cancelled = 6 // The order has been canceled.
}

public enum RefundStatus
{
    None,
    Requested,
    Completed,
    Cancelled
}

public class CreateOrderRequestWHMCS
{
    public List<CreateProductRequestWHMCS> Products { get; set; }
    public List<string> AdminAssigned { get; set; }
    public string OrderForClientId { get; set; }
    public string CustomerIP { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public string Notes { get; set; }
    public string Tenant { get; set; }
    public int InvoiceNo { get; set; }
    public decimal Total { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Credit { get; set; }
}