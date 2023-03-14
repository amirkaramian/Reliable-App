using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ManageUserApiKey.Events;

public class APIKeyPairUpdatedEvent : DomainEvent
{
    public APIKeyPairUpdatedEvent(APIKeyPair aPIKeyPair)
    {
        APIKeyPair = aPIKeyPair;
    }

    public APIKeyPair APIKeyPair { get; }
}
