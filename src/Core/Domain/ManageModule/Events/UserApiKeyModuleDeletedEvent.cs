using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ManageModule.Events;

public class UserApiKeyModuleDeletedEvent : DomainEvent
{
    public UserApiKeyModuleDeletedEvent(UserApiKeyModule module)
    {
        UserApiKeyModule = module;
    }

    public UserApiKeyModule UserApiKeyModule { get; }
}
