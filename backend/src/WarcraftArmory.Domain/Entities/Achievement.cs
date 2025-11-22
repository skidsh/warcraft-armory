namespace WarcraftArmory.Domain.Entities;

/// <summary>
/// Represents a World of Warcraft achievement (read-only data from Blizzard API).
/// </summary>
public sealed record Achievement
{
    /// <summary>
    /// Gets or sets the achievement's unique identifier.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Gets or sets the achievement's name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets the achievement's description.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Gets or sets the achievement points awarded.
    /// </summary>
    public int Points { get; init; }

    /// <summary>
    /// Gets or sets the achievement's icon name.
    /// </summary>
    public string? Icon { get; init; }

    /// <summary>
    /// Gets or sets the achievement's media URL.
    /// </summary>
    public string? MediaUrl { get; init; }

    /// <summary>
    /// Gets or sets the achievement category ID.
    /// </summary>
    public int CategoryId { get; init; }

    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    public int DisplayOrder { get; init; }

    /// <summary>
    /// Gets or sets whether this is a Feat of Strength.
    /// </summary>
    public bool IsFeatOfStrength { get; init; }

    /// <summary>
    /// Gets or sets whether this is account-wide.
    /// </summary>
    public bool IsAccountWide { get; init; }
}
