using MyReliableSite.Shared.DTOs.Storage;

namespace MyReliableSite.Shared.DTOs.CronJobs;

public class CreateCronJobsRequest : IMustBeValid
{
    public string Url { get; set; }
    public string OwnerId { get; set; }
    public string RunTime { get; set; }
    public int Status { get; set; }
    public string Tenant { get; set; }
}