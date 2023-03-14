using MyReliableSite.Application.Common.Interfaces;
using System.Security.Claims;

namespace MyReliableSite.Application.Abstractions.Services.Identity;

public interface ICurrentUser : IScopedService
{
    string Name { get; }

    Guid GetUserId();

    string GetUserEmail();

    string GetTenant();

    bool IsAuthenticated();

    bool IsInRole(string role);

    IEnumerable<Claim> GetUserClaims();

    void SetUser(ClaimsPrincipal user);

    void SetUserJob(string userId);

}