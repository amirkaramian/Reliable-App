using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.Identity;
namespace MyReliableSite.Application.Identity.Validators;

public class CreateLoginHistoryRequestValidator : CustomValidator<CreateUserLoginHistoryRequest>
{
    public CreateLoginHistoryRequestValidator()
    {
        RuleFor(p => p.Location).MaximumLength(75).NotEmpty().NotNull();
        RuleFor(p => p.DeviceName).NotEmpty().NotNull();
        RuleFor(p => p.UserId).NotEmpty().NotNull();
        RuleFor(p => p.IpAddress).NotEmpty().NotNull();
        RuleFor(p => p.LoginTime).NotEmpty().NotNull();
    }
}