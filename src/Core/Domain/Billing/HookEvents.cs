using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.Billing;

public class HookEvents : AuditableEntity, IMustHaveTenant
{
    public string Name { get; set; }
    public int Status { get; set; }
    public string Tenant { get; set; }

    public HookEvents(string name, int status, string tenant)
    {
        Name = name;
        Status = status;
        Tenant = tenant;
    }

    public HookEvents Update(string name, int status, string tenant)
    {
        if (name != Name) { Name = name; }
        if (status != Status) { Status = status; }
        if (tenant != Tenant) { Tenant = tenant; }
        return this;
    }
}