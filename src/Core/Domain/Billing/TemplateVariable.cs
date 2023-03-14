using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.Billing;

public class TemplateVariable : AuditableEntity, IMustHaveTenant
{
    public string Variable { get; set; }
    public string Description { get; set; }
    public string Tenant { get; set; }

    public TemplateVariable()
    {
    }

    public TemplateVariable(string variable, string description)
    {
        Variable = variable;
        Description = description;
    }

    public TemplateVariable Update(string variable, string description)
    {
        if (!string.IsNullOrWhiteSpace(variable) && !string.Equals(Variable, variable, StringComparison.InvariantCultureIgnoreCase)) Variable = variable;
        if (!string.IsNullOrWhiteSpace(description) && !string.Equals(Description, description, StringComparison.InvariantCultureIgnoreCase)) Description = description;
        return this;
    }
}