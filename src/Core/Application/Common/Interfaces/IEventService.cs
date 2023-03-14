using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Application.Common.Interfaces;

public interface IEventService : ITransientService
{
    Task PublishAsync(DomainEvent domainEvent);
}