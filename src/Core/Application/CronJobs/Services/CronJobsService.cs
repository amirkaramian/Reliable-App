using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.CronJobs.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.CronJobs;
using MyReliableSite.Domain.Billing.Events;

namespace MyReliableSite.Application.CronJobs.Services;

public class CronJobsService : ICronJobsService
{
    private readonly IRepositoryAsync _repository;

    public CronJobsService(IRepositoryAsync repository)
    {
        _repository = repository;
    }

    public async Task<Result<Guid>> CreateCronJobsAsync(CreateCronJobsRequest request)
    {
        var cronJobs = new Domain.Billing.CronJobs(request.Url, request.OwnerId, request.RunTime, request.Status, request.Tenant);
        cronJobs.DomainEvents.Add(new CronJobsCreatedEvent(cronJobs));
        cronJobs.DomainEvents.Add(new StatsChangedEvent());
        var settingId = await _repository.CreateAsync(cronJobs);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(settingId);
    }

    public async Task<Result<Guid>> UpdateCronJobsAsync(UpdateCronJobsRequest request, Guid id)
    {
        var cronJobs = await _repository.GetByIdAsync<Domain.Billing.CronJobs>(id, null);
        cronJobs.DomainEvents.Add(new CronJobsUpdatedEvent(cronJobs));
        cronJobs.DomainEvents.Add(new StatsChangedEvent());
        var updatedCronJobs = cronJobs.Update(request.Url, request.OwnerId, request.RunTime, request.Status, request.Tenant);

        await _repository.UpdateAsync(updatedCronJobs);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<CronJobsDetailsDto>> GetCronJobsDetailsAsync(Guid id)
    {
        var cronJobs = await _repository.GetByIdAsync<Domain.Billing.CronJobs, CronJobsDetailsDto>(id);
        return await Result<CronJobsDetailsDto>.SuccessAsync(cronJobs);
    }

    public async Task<Result<Guid>> DeleteCronJobsAsync(Guid id)
    {
        var delete = await _repository.RemoveByIdAsync<Domain.Billing.CronJobs>(id);
        delete.DomainEvents.Add(new CronJobsDeletedEvent(delete));
        delete.DomainEvents.Add(new StatsChangedEvent());
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }
}
