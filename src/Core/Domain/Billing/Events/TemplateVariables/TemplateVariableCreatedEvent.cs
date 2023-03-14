using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.TemplateVariables;

public class TemplateVariableCreatedEvent : DomainEvent
{
    public TemplateVariableCreatedEvent(TemplateVariable templateVariable)
    {
        TemplateVariable = templateVariable;
    }

    public TemplateVariable TemplateVariable { get; }
}