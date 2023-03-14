using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Billing.Events.PaymentGateways;

namespace MyReliableSite.Application.PaymentGateways.EventHandlers;

public class PaymentGatewayCreatedEventHandler : INotificationHandler<EventNotification<PaymentGatewayCreatedEvent>>
{
    private readonly ILogger<PaymentGatewayCreatedEventHandler> _logger;

    public PaymentGatewayCreatedEventHandler(ILogger<PaymentGatewayCreatedEventHandler> logger) => _logger = logger;

    public Task Handle(EventNotification<PaymentGatewayCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
