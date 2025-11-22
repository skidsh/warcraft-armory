using WarcraftArmory.Domain.Enums;

namespace WarcraftArmory.Application.DTOs.Requests;

/// <summary>
/// Request DTO for getting an item.
/// </summary>
public sealed record GetItemRequest
{
    /// <summary>
    /// Gets or sets the item ID.
    /// </summary>
    public required int ItemId { get; init; }

    /// <summary>
    /// Gets or sets the region.
    /// </summary>
    public required Region Region { get; init; }
}
