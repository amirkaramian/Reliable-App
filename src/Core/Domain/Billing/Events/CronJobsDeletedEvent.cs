using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events;

public class CronJobsDeletedEvent : DomainEvent
{
    public CronJobsDeletedEvent(CronJobs cronJobs)
    {
        CronJobs = cronJobs;
    }

    public CronJobs CronJobs { get; }
}