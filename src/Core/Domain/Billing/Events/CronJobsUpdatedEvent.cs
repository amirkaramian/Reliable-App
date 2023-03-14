using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events;

public class CronJobsUpdatedEvent : DomainEvent
{
    public CronJobsUpdatedEvent(CronJobs cronJobs)
    {
        CronJobs = cronJobs;
    }

    public CronJobs CronJobs { get; }
}