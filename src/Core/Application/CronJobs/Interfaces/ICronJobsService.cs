using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.CronJobs;

namespace MyReliableSite.Application.CronJobs.Interfaces;

public interface ICronJobsService : ITransientService
{
    Task<Result<Guid>> CreateCronJobsAsync(CreateCronJobsRequest request);

    Task<Result<Guid>> UpdateCronJobsAsync(UpdateCronJobsRequest request, Guid id);

    Task<Result<CronJobsDetailsDto>> GetCronJobsDetailsAsync(Guid id);

    Task<Result<Guid>> DeleteCronJobsAsync(Guid id);
}
