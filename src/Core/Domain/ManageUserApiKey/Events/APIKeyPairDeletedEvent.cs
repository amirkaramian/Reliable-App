using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ManageUserApiKey.Events;

public class APIKeyPairDeletedEvent : DomainEvent
{
    public APIKeyPairDeletedEvent(APIKeyPair aPIKeyPair)
    {
        APIKeyPair = aPIKeyPair;
    }

    public APIKeyPair APIKeyPair { get; }
}
