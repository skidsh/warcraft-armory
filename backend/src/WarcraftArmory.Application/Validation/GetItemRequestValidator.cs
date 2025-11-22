using FluentValidation;
using WarcraftArmory.Application.DTOs.Requests;

namespace WarcraftArmory.Application.Validation;

/// <summary>
/// Validator for GetItemRequest.
/// </summary>
public sealed class GetItemRequestValidator : AbstractValidator<GetItemRequest>
{
    public GetItemRequestValidator()
    {
        RuleFor(x => x.ItemId)
            .GreaterThan(0).WithMessage("Item ID must be greater than 0.");

        RuleFor(x => x.Region)
            .IsInEnum().WithMessage("Invalid region.");
    }
}
