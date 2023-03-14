using System.Net;

namespace MyReliableSite.Application.Exceptions;

public class EntityNotFoundException : CustomException
{
    public EntityNotFoundException(string message)
        : base(message, null, HttpStatusCode.NotFound)
    {
    }
}