using FluentValidation;

using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.EmailTemplates;

namespace MyReliableSite.Application.EmailTemplates.Validators;

public class UpdateTemplateVariableRequestValidator : CustomValidator<UpdateTemplateVariableRequest>
{
    public UpdateTemplateVariableRequestValidator()
    {
        RuleFor(p => p.Variable).MaximumLength(50).NotEmpty();
        RuleFor(p => p.Description).NotEmpty();
    }
}