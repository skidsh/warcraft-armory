using System.Collections.Immutable;

namespace WarcraftArmory.Domain.Entities;

/// <summary>
/// Represents a World of Warcraft mount (read-only data from Blizzard API).
/// </summary>
public sealed record Mount
{
    /// <summary>
    /// Gets or sets the mount's unique identifier.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Gets or sets the mount's name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets the mount's description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets or sets the creature displays associated with this mount.
    /// Uses ImmutableArray for optimal performance with record types.
    /// </summary>
    public ImmutableArray<int> CreatureDisplays { get; init; } = [];

    /// <summary>
    /// Gets or sets whether the mount is ground-only.
    /// </summary>
    public bool IsGroundMount { get; init; }

    /// <summary>
    /// Gets or sets whether the mount can fly.
    /// </summary>
    public bool IsFlying { get; init; }

    /// <summary>
    /// Gets or sets whether the mount is aquatic.
    /// </summary>
    public bool IsAquatic { get; init; }

    /// <summary>
    /// Gets or sets whether the mount is a dragonriding mount.
    /// </summary>
    public bool IsDragonriding { get; init; }

    /// <summary>
    /// Gets or sets the source type (e.g., Drop, Quest, Vendor).
    /// </summary>
    public string? SourceType { get; init; }

    /// <summary>
    /// Gets or sets the source description.
    /// </summary>
    public string? SourceDescription { get; init; }
}
