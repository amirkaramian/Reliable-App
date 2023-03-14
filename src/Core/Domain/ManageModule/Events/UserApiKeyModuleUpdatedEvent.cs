using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ManageModule.Events;

public class UserApiKeyModuleUpdatedEvent : DomainEvent
{
    public UserApiKeyModuleUpdatedEvent(UserApiKeyModule module)
    {
        UserApiKeyModule = module;
    }

    public UserApiKeyModule UserApiKeyModule { get; }
}
