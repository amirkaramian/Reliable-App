using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.WebHooksDomain.Events;

public class WebHookCreatedEvent : DomainEvent
{
    public WebHookCreatedEvent(WebHook webHook)
    {
        WebHook = webHook;
    }

    public WebHook WebHook { get; }
}