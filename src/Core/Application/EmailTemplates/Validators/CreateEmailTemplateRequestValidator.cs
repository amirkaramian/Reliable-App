using FluentValidation;

using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.EmailTemplates;

namespace MyReliableSite.Application.EmailTemplates.Validators;

public class CreateEmailTemplateRequestValidator : CustomValidator<CreateEmailTemplateRequest>
{
    public CreateEmailTemplateRequestValidator()
    {
        RuleFor(p => p.Subject).MaximumLength(100).NotEmpty();
        RuleFor(p => p.Body).NotEmpty();
        RuleFor(p => p.IsSystem).Must(x => x == true || x == false);
        RuleFor(p => p.EmailTemplateType).IsInEnum();
    }
}