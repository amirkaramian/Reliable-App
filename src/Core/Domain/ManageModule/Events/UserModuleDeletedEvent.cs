using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ManageModule.Events;

public class UserModuleDeletedEvent : DomainEvent
{
    public UserModuleDeletedEvent(UserModule module)
    {
        UserModule = module;
    }

    public UserModule UserModule { get; }
}
