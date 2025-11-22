using WarcraftArmory.Domain.Enums;

namespace WarcraftArmory.Domain.Entities;

/// <summary>
/// Represents a World of Warcraft character (read-only data from Blizzard API).
/// </summary>
public sealed record Character
{
    /// <summary>
    /// Gets or sets the character's unique identifier.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Gets or sets the character's name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets the realm the character belongs to.
    /// </summary>
    public required string Realm { get; init; }

    /// <summary>
    /// Gets or sets the region the character is in.
    /// </summary>
    public required Region Region { get; init; }

    /// <summary>
    /// Gets or sets the character's class.
    /// </summary>
    public required CharacterClass Class { get; init; }

    /// <summary>
    /// Gets or sets the character's race.
    /// </summary>
    public required CharacterRace Race { get; init; }

    /// <summary>
    /// Gets or sets the character's level.
    /// </summary>
    public required int Level { get; init; }

    /// <summary>
    /// Gets or sets the character's faction.
    /// </summary>
    public required Faction Faction { get; init; }

    /// <summary>
    /// Gets or sets the character's gender.
    /// </summary>
    public required Gender Gender { get; init; }

    /// <summary>
    /// Gets or sets the character's achievement points.
    /// </summary>
    public int AchievementPoints { get; init; }

    /// <summary>
    /// Gets or sets the character's average item level.
    /// </summary>
    public double AverageItemLevel { get; init; }

    /// <summary>
    /// Gets or sets the character's equipped item level.
    /// </summary>
    public double EquippedItemLevel { get; init; }

    /// <summary>
    /// Gets or sets the character's thumbnail URL.
    /// </summary>
    public string? ThumbnailUrl { get; init; }

    /// <summary>
    /// Gets or sets the guild ID this character belongs to (if any).
    /// </summary>
    public int? GuildId { get; init; }

    /// <summary>
    /// Gets or sets the last modified timestamp.
    /// </summary>
    public DateTime LastModified { get; init; }
}
