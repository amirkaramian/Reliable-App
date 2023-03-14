using MyReliableSite.Application.Exceptions;
using System.Net;

namespace MyReliableSite.Application.Multitenancy;

public class InvalidTenantException : CustomException
{
    public InvalidTenantException(string message)
        : base(message, null, HttpStatusCode.BadRequest)
    {
    }
}