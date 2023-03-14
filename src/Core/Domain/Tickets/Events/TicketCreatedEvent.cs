using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Tickets.Events;

public class TicketCreatedEvent : DomainEvent
{
    public TicketCreatedEvent(Ticket ticket)
    {
        Ticket = ticket;
    }

    public Ticket Ticket { get; }
}
