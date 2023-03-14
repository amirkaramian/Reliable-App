using FluentValidation;
using MyReliableSite.Shared.DTOs.Notifications.Templates;

namespace MyReliableSite.Application.Common.Validators;

public class CreateNotificationTemplateRequestValidator : CustomValidator<CreateNotificationTemplateRequest>
{
    public CreateNotificationTemplateRequestValidator()
    {
        RuleFor(p => p.Title).MaximumLength(100).NotEmpty();
        RuleFor(p => p.Body).NotEmpty();
        RuleFor(p => p.Status).IsInEnum();
        RuleFor(p => p.TargetUserType).IsInEnum();
    }
}