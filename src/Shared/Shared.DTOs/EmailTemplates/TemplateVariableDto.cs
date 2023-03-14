namespace MyReliableSite.Shared.DTOs.EmailTemplates;

public class TemplateVariableDto : IDto
{
    public string Variable { get; set; }
    public string Description { get; set; }
    public string Tenant { get; set; }
}
