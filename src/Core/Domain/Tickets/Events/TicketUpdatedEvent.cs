using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Tickets.Events;

public class TicketUpdatedEvent : DomainEvent
{
    public TicketUpdatedEvent(Ticket ticket)
    {
        Ticket = ticket;
    }

    public Ticket Ticket { get; }
}
