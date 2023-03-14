using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ManageModule.Events;

public class UserModuleCreatedEvent : DomainEvent
{
    public UserModuleCreatedEvent(UserModule module)
    {
        UserModule = module;
    }

    public UserModule UserModule { get; }
}
