using WarcraftArmory.Domain.Enums;

namespace WarcraftArmory.Application.DTOs.Requests;

/// <summary>
/// Request DTO for getting a character.
/// </summary>
public sealed record GetCharacterRequest
{
    /// <summary>
    /// Gets or sets the realm slug.
    /// </summary>
    public required string Realm { get; init; }

    /// <summary>
    /// Gets or sets the character name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets the region.
    /// </summary>
    public required Region Region { get; init; }
}
