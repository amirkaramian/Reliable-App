using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.WebHooksDomain.Events;

public class WebHookUpdatedEvent : DomainEvent
{
    public WebHookUpdatedEvent(WebHook webHook)
    {
        WebHook = webHook;
    }

    public WebHook WebHook { get; }
}