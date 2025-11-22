using FluentValidation;
using WarcraftArmory.Application.DTOs.Requests;

namespace WarcraftArmory.Application.Validation;

/// <summary>
/// Validator for GetCharacterRequest.
/// </summary>
public sealed class GetCharacterRequestValidator : AbstractValidator<GetCharacterRequest>
{
    public GetCharacterRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Character name is required.")
            .MinimumLength(2).WithMessage("Character name must be at least 2 characters.")
            .MaximumLength(12).WithMessage("Character name must not exceed 12 characters.")
            .Matches(@"^[a-zA-Z\u00C0-\u017F]+$").WithMessage("Character name can only contain letters.");

        RuleFor(x => x.Realm)
            .NotEmpty().WithMessage("Realm is required.")
            .MinimumLength(2).WithMessage("Realm name must be at least 2 characters.")
            .MaximumLength(50).WithMessage("Realm name must not exceed 50 characters.")
            .Matches(@"^[a-z0-9-]+$").WithMessage("Realm slug must be lowercase alphanumeric with hyphens.");

        RuleFor(x => x.Region)
            .IsInEnum().WithMessage("Invalid region.");
    }
}
