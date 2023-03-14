using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.TemplateVariables;

public class TemplateVariableDeletedEvent : DomainEvent
{
    public TemplateVariableDeletedEvent(TemplateVariable templateVariable)
    {
        TemplateVariable = templateVariable;
    }

    public TemplateVariable TemplateVariable { get; }
}