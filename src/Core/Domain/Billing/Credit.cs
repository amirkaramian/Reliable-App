using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.Billing;

public class Credit : AuditableEntity, IMustHaveTenant
{
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public string Tenant { get; set; }
    public string Description { get; set; }
    public Guid UserId { get; set; }
    public byte CreditTransactionType { get; set; }
    public Credit(Guid userId, decimal amount, DateTime dueDate, string description, byte creditTransactionType = (byte)CreditTransactionTypes.Increase)
    {
        UserId = userId;
        Amount = amount;
        DueDate = dueDate;
        Description = description;
        CreditTransactionType = (byte)creditTransactionType;
    }

    public Credit Update(decimal amount, DateTime dueDate, string description)
    {
        if (Amount != amount) Amount = amount;
        if (DueDate != dueDate) DueDate = dueDate;
        if (Description != description) Description = description;
        return this;
    }

    public enum CreditTransactionTypes
    {
        Increase = 1,
        Decrease = 2,
    }

}
