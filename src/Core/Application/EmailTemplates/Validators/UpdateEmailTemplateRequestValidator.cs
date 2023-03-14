using FluentValidation;

using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.EmailTemplates;

namespace MyReliableSite.Application.EmailTemplates.Validators;

public class UpdateEmailTemplateRequestValidator : CustomValidator<UpdateEmailTemplateRequest>
{
    public UpdateEmailTemplateRequestValidator()
    {
        RuleFor(p => p.Subject).MaximumLength(100).NotEmpty();
        RuleFor(p => p.Body).NotEmpty().NotNull();
        RuleFor(p => p.IsSystem).Must(x => x == true || x == false);
        RuleFor(p => p.EmailTemplateType).IsInEnum();
    }
}