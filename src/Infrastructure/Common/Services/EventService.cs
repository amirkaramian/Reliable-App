using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Scripting.EventHandlers;
using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Infrastructure.Common.Services;

public class EventService : IEventService
{
    private readonly ILogger<EventService> _logger;
    private readonly IPublisher _mediator;

    public EventService(ILogger<EventService> logger, IPublisher mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task PublishAsync(DomainEvent @event)
    {
        _logger.LogInformation("Publishing Event : {event}", @event.GetType().Name);
        var global = new GlobalEvent(@event);
        await _mediator.Publish(GetEventNotification(global));
        await _mediator.Publish(GetEventNotification(@event));
    }

    private INotification GetEventNotification(DomainEvent @event)
    {
        return (INotification)Activator.CreateInstance(
            typeof(EventNotification<>).MakeGenericType(@event.GetType()), @event)!;
    }
}