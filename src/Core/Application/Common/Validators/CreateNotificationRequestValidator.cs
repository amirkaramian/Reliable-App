using FluentValidation;
using MyReliableSite.Shared.DTOs.Notifications;

namespace MyReliableSite.Application.Common.Validators;

public class CreateNotificationRequestValidator : CustomValidator<SendBasicNotificationRequest>
{
    public CreateNotificationRequestValidator()
    {
        RuleFor(p => p.ToUserIds).NotEmpty().NotNull();
        RuleFor(p => p.NotificationTemplateId).NotEmpty().NotNull();
        RuleFor(p => p.TargetUserTypes).IsInEnum();
    }
}