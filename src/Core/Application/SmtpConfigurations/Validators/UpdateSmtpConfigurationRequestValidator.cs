using FluentValidation;

using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.SmtpConfigurations;

namespace MyReliableSite.Application.SmtpConfigurations.Validators;

public class UpdateSmtpConfigurationRequestValidator : CustomValidator<UpdateSmtpConfigurationRequest>
{
    public UpdateSmtpConfigurationRequestValidator()
    {
        RuleFor(p => p.Port).Must(p => p.ToString().Length >= 3);
        RuleFor(p => p.Host).MaximumLength(50).NotEmpty();
        RuleFor(p => p.FromName).MaximumLength(50).NotEmpty();
        RuleFor(p => p.FromEmail).MaximumLength(50).NotEmpty();
        RuleFor(p => p.Signature).MaximumLength(100).NotEmpty();
        RuleFor(p => p.BrandSmtpConfigurations).NotEmpty().NotNull();
        RuleFor(p => p.Username).NotEmpty().NotNull();
        RuleFor(p => p.Password).NotEmpty().NotNull();
    }
}