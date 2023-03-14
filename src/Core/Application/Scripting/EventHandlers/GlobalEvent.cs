using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Application.Scripting.EventHandlers;

public class GlobalEvent : DomainEvent
{
    public object Data { get; set; }

    public GlobalEvent(object Data)
    {
        this.Data = Data;
    }
}
