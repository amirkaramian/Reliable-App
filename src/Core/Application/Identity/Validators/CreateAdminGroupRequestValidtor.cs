using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Application.Identity.Validators;

public class CreateAdminGroupRequestValidtor : CustomValidator<CreateAdminGroupRequest>
{
    public CreateAdminGroupRequestValidtor()
    {
        RuleFor(p => p.GroupName).MaximumLength(75).NotEmpty();
        RuleFor(p => p.Tenant).NotEmpty();
        RuleFor(p => p.Status).Must(x => x == false || x == true);
        RuleFor(p => p.IsDefault).Must(x => x == false || x == true);
        RuleFor(p => p.IsSuperAdmin).Must(x => x == false || x == true);

    }
}
