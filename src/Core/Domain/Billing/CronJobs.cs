using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.Billing;

public class CronJobs : AuditableEntity, IMustHaveTenant
{
    public string Url { get; set; }
    public string OwnerId { get; set; }
    public string RunTime { get; set; }
    public int Status { get; set; }
    public string Tenant { get; set; }

    public CronJobs(string url, string ownerId, string runTime, int status, string tenant)
    {
        Url = url;
        OwnerId = ownerId;
        RunTime = runTime;
        Status = status;
        Tenant = tenant;
    }

    public CronJobs Update(string url, string ownerId, string runTime, int status, string tenant)
    {
        if (url != Url) { Url = url; }
        if (ownerId != OwnerId) { OwnerId = ownerId; }
        if (runTime != RunTime) { RunTime = runTime; }
        if (status != Status) { Status = status; }
        if (tenant != Tenant) { Tenant = tenant; }
        return this;
    }
}