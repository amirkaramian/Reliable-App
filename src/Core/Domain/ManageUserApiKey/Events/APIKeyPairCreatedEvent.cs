using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ManageUserApiKey.Events;

public class APIKeyPairCreatedEvent : DomainEvent
{
    public APIKeyPairCreatedEvent(APIKeyPair aPIKeyPair)
    {
        APIKeyPair = aPIKeyPair;
    }

    public APIKeyPair APIKeyPair { get; }
}
