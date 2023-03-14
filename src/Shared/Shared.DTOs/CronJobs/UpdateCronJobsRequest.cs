namespace MyReliableSite.Shared.DTOs.CronJobs;

public class UpdateCronJobsRequest : IMustBeValid
{
    public string Url { get; set; }
    public string OwnerId { get; set; }
    public string RunTime { get; set; }
    public int Status { get; set; }
    public string Tenant { get; set; }
}