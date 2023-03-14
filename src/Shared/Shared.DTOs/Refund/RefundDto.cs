using MyReliableSite.Shared.DTOs.Orders;

namespace MyReliableSite.Shared.DTOs.Refund;
public class RefundDto : IDto
{
    public int RefundNo { get; set; }
    public Guid RequestById { get; set; }
    public Guid? ActionTakenById { get; set; }
    public string Notes { get; set; }
    public decimal Total { get; set; } = 0;
    public decimal TotalAfterRetainPercentage { get; set; } = 0;
    public Guid OrderId { get; set; }
    public RefundStatus RefundStatus { get; set; }
    public string Tenant { get; set; }
}
