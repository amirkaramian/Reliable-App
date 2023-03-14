using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Tickets.Events;

public class TicketDeletedEvent : DomainEvent
{
    public TicketDeletedEvent(Ticket ticket)
    {
        Ticket = ticket;
    }

    public Ticket Ticket { get; }
}
