using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.Billing;

public class Bill : AuditableEntity, IMustHaveTenant
{
    public string BillNo { get; set; }
    public string UserId { get; set; }
    public Guid OrderId { get; set; }
    public virtual Order Order { get; set; }
    public DateTime DueDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal VAT { get; set; }
    public string Tenant { get; set; }

    public Bill()
    {

    }

    public Bill(string billNo, string userId, decimal vat, DateTime orderDueDate, decimal subTotal)
    {
        BillNo = billNo;
        UserId = userId;
        DueDate = orderDueDate;
        VAT = vat;
        SubTotal = subTotal;
    }

    public Bill(string billNo, string userId, decimal vat, DateTime dueDate, Guid orderId, decimal subTotal)
    {
        BillNo = billNo;
        UserId = userId;
        DueDate = dueDate;
        VAT = vat;
        OrderId = orderId;
        SubTotal = subTotal;
    }

    public Bill Update(string billNo)
    {
        BillNo = billNo;
        return this;
    }

    public Bill Update(string billNo, string userId, DateTime dueDate, decimal vat, decimal subTotal)
    {
        if (dueDate != DueDate) DueDate = dueDate;
        if (billNo != null && !BillNo.NullToString().Equals(billNo)) BillNo = billNo;
        if (userId != null && !UserId.NullToString().Equals(userId)) UserId = userId;
        if (VAT != vat) VAT = vat;
        if(SubTotal != subTotal) SubTotal = subTotal;
        return this;
    }
}