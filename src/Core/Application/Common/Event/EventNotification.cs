using MediatR;
using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Application.Common.Event;

public class EventNotification<T> : INotification
    where T : DomainEvent
{
    public EventNotification(T domainEvent)
    {
        DomainEvent = domainEvent;
    }

    public T DomainEvent { get; }
}