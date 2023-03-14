using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Application.Storage;
using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Application.Validators.Identity;

public class UpdateProfileRequestValidator : CustomValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        string[] pattern = new string[]
      {
            "^",                                            // Start of string
            @"([01]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])\.",    // Between 000 and 255 and "."
            @"([01]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])\.",
            @"([01]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])\.",
            @"([01]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])",      // Same as before, no period
            "$",                                            // End of string
      };
        RuleFor(p => p.FullName).MaximumLength(150).NotEmpty().NotNull();

        // RuleFor(p => p.IpAddress).Matches(string.Join(string.Empty, pattern));
        RuleFor(p => p.Image).SetValidator(new FileUploadRequestValidator());
        RuleFor(p => p.Status).Must(x => x == false || x == true);
    }
}