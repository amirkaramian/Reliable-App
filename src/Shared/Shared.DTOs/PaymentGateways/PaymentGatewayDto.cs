using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.PaymentGateways;

public class PaymentGatewayDto : IDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string ApiKey { get; set; }
    public bool Status { get; set; }
    public string Tenant { get; set; }
}

