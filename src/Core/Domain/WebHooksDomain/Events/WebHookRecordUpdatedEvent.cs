using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.WebHooksDomain.Events;

public class WebHookRecordUpdatedEvent : DomainEvent
{
    public WebHookRecordUpdatedEvent(WebHookRecord webHookRecord)
    {
        WebHookRecord = webHookRecord;
    }

    public WebHookRecord WebHookRecord { get; }
}