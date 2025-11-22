using WarcraftArmory.Domain.Enums;

namespace WarcraftArmory.Domain.Entities;

/// <summary>
/// Represents a World of Warcraft guild (read-only data from Blizzard API).
/// </summary>
public sealed record Guild
{
    /// <summary>
    /// Gets or sets the guild's unique identifier.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Gets or sets the guild's name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets the realm the guild is on.
    /// </summary>
    public required string Realm { get; init; }

    /// <summary>
    /// Gets or sets the region the guild is in.
    /// </summary>
    public required Region Region { get; init; }

    /// <summary>
    /// Gets or sets the guild's faction.
    /// </summary>
    public required Faction Faction { get; init; }

    /// <summary>
    /// Gets or sets the guild's achievement points.
    /// </summary>
    public int AchievementPoints { get; init; }

    /// <summary>
    /// Gets or sets the number of members in the guild.
    /// </summary>
    public int MemberCount { get; init; }

    /// <summary>
    /// Gets or sets the guild's emblem data.
    /// </summary>
    public string? EmblemData { get; init; }

    /// <summary>
    /// Gets or sets the guild's created timestamp.
    /// </summary>
    public DateTime CreatedTimestamp { get; init; }

    /// <summary>
    /// Gets or sets the last modified timestamp.
    /// </summary>
    public DateTime LastModified { get; init; }
}
