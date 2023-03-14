using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.WebHooksDomain.Events;

public class WebHookRecordCreatedEvent : DomainEvent
{
    public WebHookRecordCreatedEvent(WebHookRecord webHookRecord)
    {
        WebHookRecord = webHookRecord;
    }

    public WebHookRecord WebHookRecord { get; }
}