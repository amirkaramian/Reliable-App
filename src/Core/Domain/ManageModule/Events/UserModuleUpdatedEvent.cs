using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ManageModule.Events;

public class UserModuleUpdatedEvent : DomainEvent
{
    public UserModuleUpdatedEvent(UserModule module)
    {
        UserModule = module;
    }

    public UserModule UserModule { get; }
}
