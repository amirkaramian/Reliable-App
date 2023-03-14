using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.CreditManagement;
public class MakeAllInvoiceCreditPaymentRequest : IMustBeValid
{
    public string Tenant { get; set; }
    public string Notes { get; set; }
}
