using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.ManageUserApiKey;

namespace MyReliableSite.Application.ManageUserApiKey.Validators;

public class CreateAPIKeyPairRequestValidator : CustomValidator<CreateAPIKeyPairRequest>
{
    public CreateAPIKeyPairRequestValidator()
    {
        RuleFor(p => p.ApplicationKey).NotEmpty();
        RuleFor(p => p.UserIds).NotEmpty();
        RuleFor(p => p.Tenant).NotEmpty();
        RuleFor(p => p.SafeListIpAddresses).NotEmpty();
        RuleFor(p => p.UserApiKeyModules).NotNull();
        RuleFor(p => p.ValidTill).NotNull().NotEmpty();
        RuleFor(p => p.StatusApi).Must(x => x == false || x == true);
    }
}
