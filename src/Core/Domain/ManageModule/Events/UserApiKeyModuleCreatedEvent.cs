using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ManageModule.Events;

public class UserApiKeyModuleCreatedEvent : DomainEvent
{
    public UserApiKeyModuleCreatedEvent(UserApiKeyModule module)
    {
        UserApiKeyModule = module;
    }

    public UserApiKeyModule UserApiKeyModule { get; }
}
