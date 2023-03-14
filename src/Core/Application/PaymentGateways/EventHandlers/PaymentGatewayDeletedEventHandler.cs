using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Billing.Events.PaymentGateways;

namespace MyReliableSite.Application.PaymentGateways.EventHandlers;

public class PaymentGatewayDeletedEventHandler : INotificationHandler<EventNotification<PaymentGatewayDeletedEvent>>
{
    private readonly ILogger<PaymentGatewayDeletedEventHandler> _logger;

    public PaymentGatewayDeletedEventHandler(ILogger<PaymentGatewayDeletedEventHandler> logger) => _logger = logger;

    public Task Handle(EventNotification<PaymentGatewayDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}