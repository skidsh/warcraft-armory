using WarcraftArmory.Domain.Enums;

namespace WarcraftArmory.Domain.Entities;

/// <summary>
/// Represents a World of Warcraft realm/server (read-only data from Blizzard API).
/// </summary>
public sealed record Realm
{
    /// <summary>
    /// Gets or sets the realm's unique identifier.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Gets or sets the realm's name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets the realm's slug (URL-safe name).
    /// </summary>
    public required string Slug { get; init; }

    /// <summary>
    /// Gets or sets the region the realm is in.
    /// </summary>
    public required Region Region { get; init; }

    /// <summary>
    /// Gets or sets the realm's timezone.
    /// </summary>
    public required string Timezone { get; init; }

    /// <summary>
    /// Gets or sets the realm's locale.
    /// </summary>
    public required string Locale { get; init; }

    /// <summary>
    /// Gets or sets whether the realm is a tournament realm.
    /// </summary>
    public bool IsTournament { get; init; }

    /// <summary>
    /// Gets or sets the realm's category (e.g., "us").
    /// </summary>
    public required string Category { get; init; }

    /// <summary>
    /// Gets or sets the realm type (e.g., Normal, PvP, RP).
    /// </summary>
    public string? Type { get; init; }
}
