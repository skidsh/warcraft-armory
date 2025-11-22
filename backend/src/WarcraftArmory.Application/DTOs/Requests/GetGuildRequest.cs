using WarcraftArmory.Domain.Enums;

namespace WarcraftArmory.Application.DTOs.Requests;

/// <summary>
/// Request DTO for getting a guild.
/// </summary>
public sealed record GetGuildRequest
{
    /// <summary>
    /// Gets or sets the realm slug.
    /// </summary>
    public required string Realm { get; init; }

    /// <summary>
    /// Gets or sets the guild name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets the region.
    /// </summary>
    public required Region Region { get; init; }
}
