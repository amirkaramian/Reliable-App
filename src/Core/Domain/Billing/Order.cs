using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using MyReliableSite.Domain.Products;
using MyReliableSite.Shared.DTOs.Orders;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyReliableSite.Domain.Billing;

public class Order : AuditableEntity, IMustHaveTenant
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OrderNo { get; set; }
    public string ClientId { get; set; }
    public string CustomerIP { get; set; }
    public string Notes { get; set; }
    public OrderStatus Status { get; set; }
    public string Tenant { get; set; }
    public string OldOrderId { get; set; }
    public string InvoiceNo { get; set; }
    public decimal Total { get; set; }

    [NotMapped]
    public List<string> AdminAssigned
    {
        get => AdminAssignedString.Split(";").ToList();
        set => AdminAssignedString = string.Join(";", value);
    }

    public virtual Bill Bill { get; set; }
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    public virtual ICollection<OrderProductLineItem> OrderProductLineItems { get; set; } = new List<OrderProductLineItem>();
    public bool Notify { get; set; } = false;
    public string AdminAssignedString { get; set; } = string.Empty;

    public Order()
    {

    }

    public Order(string clientId, List<string> adminAssigned, string customerIP, string notes, OrderStatus status, decimal total, bool notify = false)
    {
        ClientId = clientId;
        AdminAssigned = adminAssigned;
        CustomerIP = customerIP;
        Notes = notes;
        Status = status;
        Total = total;
        Notify = notify;
    }

    public Order Update(string notes, OrderStatus status, List<string> adminAssignedId)
    {
        if (notes != null && !Notes.NullToString().Equals(notes)) Notes = notes;
        if (adminAssignedId != null && !AdminAssigned.Equals(adminAssignedId)) // AdminAssigned = adminAssignedId;
            AdminAssigned = adminAssignedId;
        if (status != Status) Status = status;
        return this;
    }

    public Order Update(List<string> adminAssignedId)
    {
        if (adminAssignedId != null && !AdminAssigned.Equals(adminAssignedId)) // AdminAssigned = adminAssignedId;
            AdminAssigned = adminAssignedId;
        return this;
    }
}
