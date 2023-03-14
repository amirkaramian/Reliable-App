namespace MyReliableSite.Shared.DTOs.CronJobs;

public class CronJobsDto : IDto
{
    public Guid Id { get; set; }
    public string Url { get; set; }
    public string OwnerId { get; set; }
    public string RunTime { get; set; }
    public int Status { get; set; }
    public string Tenant { get; set; }
}