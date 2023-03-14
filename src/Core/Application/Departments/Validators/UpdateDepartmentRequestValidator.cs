using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.Departments;

namespace MyReliableSite.Application.Departments.Validators;

public class UpdateDepartmentRequestValidator : CustomValidator<UpdateDepartmentRequest>
{
    public UpdateDepartmentRequestValidator()
    {
        RuleFor(p => p.BrandId).NotEmpty().NotNull();
        RuleFor(p => p.DeptNumber).NotEmpty();
        RuleFor(p => p.Name).NotEmpty();
        RuleFor(p => p.DeptStatus).Must(x => x == true || x == false);
        RuleFor(p => p.IsDefault).Must(x => x == true || x == false);
    }
}
