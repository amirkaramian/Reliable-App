using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Shared.DTOs.Multitenancy;

namespace MyReliableSite.Application.Multitenancy;

public interface ITenantService : IScopedService
{
    public string GetDatabaseProvider();

    public string GetConnectionString();

    public TenantDto GetCurrentTenant();

    public void SetCurrentTenant(string tenant);
}