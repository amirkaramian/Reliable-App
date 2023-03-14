using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.ManageModule;

namespace MyReliableSite.Application.ManageModule.Validators;

public class CreateModuleRequestValidator : CustomValidator<CreateModuleManagementRequest>
{
    public CreateModuleRequestValidator()
    {
        RuleFor(p => p.Name).MaximumLength(75).NotEmpty();
        RuleFor(p => p.PermissionDetail).NotEmpty();
        RuleFor(p => p.Tenant).NotEmpty();
        RuleFor(p => p.IsActive).Must(x => x == false || x == true);
    }
}
