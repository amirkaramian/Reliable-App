using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.WebHooksDomain.Events;

public class WebHookRecordDeletedEvent : DomainEvent
{
    public WebHookRecordDeletedEvent(WebHookRecord webHookRecord)
    {
        WebHookRecord = webHookRecord;
    }

    public WebHookRecord WebHookRecord { get; }
}