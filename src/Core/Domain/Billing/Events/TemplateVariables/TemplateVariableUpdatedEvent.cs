using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.TemplateVariables;

public class TemplateVariableUpdatedEvent : DomainEvent
{
    public TemplateVariableUpdatedEvent(TemplateVariable templateVariable)
    {
        TemplateVariable = templateVariable;
    }

    public TemplateVariable TemplateVariable { get; }
}