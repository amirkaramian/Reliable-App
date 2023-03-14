using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Billing.Events.PaymentGateways;

namespace MyReliableSite.Application.PaymentGateways.EventHandlers;

public class PaymentGatewayUpdatedEventHandler : INotificationHandler<EventNotification<PaymentGatewayUpdatedEvent>>
{
    private readonly ILogger<PaymentGatewayUpdatedEventHandler> _logger;

    public PaymentGatewayUpdatedEventHandler(ILogger<PaymentGatewayUpdatedEventHandler> logger) => _logger = logger;

    public Task Handle(EventNotification<PaymentGatewayUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}