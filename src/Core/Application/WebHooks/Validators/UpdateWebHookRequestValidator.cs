using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.WebHooks;

namespace MyReliableSite.Application.WebHooks.Validators;

public class UpdateWebHookRequestValidator : CustomValidator<UpdateWebHooksRequest>
{
    public UpdateWebHookRequestValidator()
    {
        RuleFor(e => e.WebHookUrl)
           .NotEmpty()
           .NotNull();

        RuleFor(e => e.Action)
        .IsInEnum();
        RuleFor(p => p.ModuleId).NotEmpty().NotNull();
        RuleFor(p => p.IsActive).Must(x => x == false || x == true);

    }
}