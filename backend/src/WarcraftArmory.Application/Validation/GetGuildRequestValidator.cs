using FluentValidation;
using WarcraftArmory.Application.DTOs.Requests;

namespace WarcraftArmory.Application.Validation;

/// <summary>
/// Validator for GetGuildRequest.
/// </summary>
public sealed class GetGuildRequestValidator : AbstractValidator<GetGuildRequest>
{
    public GetGuildRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Guild name is required.")
            .MinimumLength(2).WithMessage("Guild name must be at least 2 characters.")
            .MaximumLength(24).WithMessage("Guild name must not exceed 24 characters.");

        RuleFor(x => x.Realm)
            .NotEmpty().WithMessage("Realm is required.")
            .MinimumLength(2).WithMessage("Realm name must be at least 2 characters.")
            .MaximumLength(50).WithMessage("Realm name must not exceed 50 characters.")
            .Matches(@"^[a-z0-9-]+$").WithMessage("Realm slug must be lowercase alphanumeric with hyphens.");

        RuleFor(x => x.Region)
            .IsInEnum().WithMessage("Invalid region.");
    }
}
