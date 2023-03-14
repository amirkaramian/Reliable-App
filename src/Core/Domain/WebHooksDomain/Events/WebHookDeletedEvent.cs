using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.WebHooksDomain.Events;

public class WebHookDeletedEvent : DomainEvent
{
    public WebHookDeletedEvent(WebHook webHook)
    {
        WebHook = webHook;
    }

    public WebHook WebHook { get; }
}