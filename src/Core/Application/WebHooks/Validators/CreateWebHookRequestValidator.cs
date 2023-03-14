using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.WebHooks;

namespace MyReliableSite.Application.WebHooks.Validators;

public class CreateWebHookRequestValidator : CustomValidator<CreateWebHooksRequest>
{
    public CreateWebHookRequestValidator()
    {
        RuleFor(e => e.WebHookUrl)
           .NotEmpty()
           .NotNull();

        RuleFor(e => e.Action)
        .IsInEnum();

        RuleFor(e => e.ModuleId)
          .NotEmpty()
          .NotNull();

        RuleFor(p => p.IsActive).Must(x => x == false || x == true);
    }
}