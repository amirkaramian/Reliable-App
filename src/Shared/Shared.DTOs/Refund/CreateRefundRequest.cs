using MyReliableSite.Shared.DTOs.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Refund;
public class CreateRefundRequest : IMustBeValid
{
    public Guid OrderId { get; set; }
    public string Notes { get; set; }
    public decimal TotalAfterRetainPercentage { get; set; } = 0;
    public RefundStatus RefundStatus { get; set; }
    public Guid RequestById { get; set; }
    public Guid? ActionTakenById { get; set; }
    public string Tenant { get; set; }

}
