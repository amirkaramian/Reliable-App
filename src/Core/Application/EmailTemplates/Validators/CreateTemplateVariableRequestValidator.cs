using FluentValidation;

using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.EmailTemplates;

namespace MyReliableSite.Application.EmailTemplates.Validators;

public class CreateTemplateVariableRequestValidator : CustomValidator<CreateTemplateVariableRequest>
{
    public CreateTemplateVariableRequestValidator()
    {
        RuleFor(p => p.Variable).MaximumLength(50).NotEmpty();
        RuleFor(p => p.Description).NotEmpty();
    }
}