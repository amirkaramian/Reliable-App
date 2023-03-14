using MyReliableSite.Application.Exceptions;
using System.Net;

namespace MyReliableSite.Application.Identity.Exceptions;

public class IdentityException : CustomException
{
    public IdentityException(string message, List<string> errors = default, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        : base(message, errors, statusCode)
    {
    }
}