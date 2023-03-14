using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.PaymentGateways;

public class PaymentGatewayCreatedEvent : DomainEvent
{
    public PaymentGatewayCreatedEvent(PaymentGateway paymentGateway)
    {
        PaymentGateway = paymentGateway;
    }

    public PaymentGateway PaymentGateway { get; }
}