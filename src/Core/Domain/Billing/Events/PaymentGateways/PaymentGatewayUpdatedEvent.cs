using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.PaymentGateways;

public class PaymentGatewayUpdatedEvent : DomainEvent
{
    public PaymentGatewayUpdatedEvent(PaymentGateway paymentGateway)
    {
        PaymentGateway = paymentGateway;
    }

    public PaymentGateway PaymentGateway { get; }
}