using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.PaymentGateways;

public class PaymentGatewayDeletedEvent : DomainEvent
{
    public PaymentGatewayDeletedEvent(PaymentGateway paymentGateway)
    {
        PaymentGateway = paymentGateway;
    }

    public PaymentGateway PaymentGateway { get; }
}