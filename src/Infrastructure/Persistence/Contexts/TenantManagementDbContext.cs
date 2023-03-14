using Microsoft.EntityFrameworkCore;
using MyReliableSite.Domain.Multitenancy;

namespace MyReliableSite.Infrastructure.Persistence.Contexts;

public class TenantManagementDbContext : DbContext
{
    public TenantManagementDbContext(DbContextOptions<TenantManagementDbContext> options)
    : base(options)
    {
    }

    public DbSet<Tenant> Tenants { get; set; }
}
