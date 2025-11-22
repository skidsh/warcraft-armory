using WarcraftArmory.Domain.Enums;

namespace WarcraftArmory.Domain.Entities;

/// <summary>
/// Represents a World of Warcraft item (read-only data from Blizzard API).
/// </summary>
public sealed record Item
{
    /// <summary>
    /// Gets or sets the item's unique identifier.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Gets or sets the item's name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets the item's quality/rarity.
    /// </summary>
    public required ItemQuality Quality { get; init; }

    /// <summary>
    /// Gets or sets the item's level.
    /// </summary>
    public required int Level { get; init; }

    /// <summary>
    /// Gets or sets the required level to use the item.
    /// </summary>
    public int RequiredLevel { get; init; }

    /// <summary>
    /// Gets or sets the item class (e.g., Weapon, Armor).
    /// </summary>
    public required int ItemClass { get; init; }

    /// <summary>
    /// Gets or sets the item subclass.
    /// </summary>
    public required int ItemSubclass { get; init; }

    /// <summary>
    /// Gets or sets the inventory type (slot).
    /// </summary>
    public required int InventoryType { get; init; }

    /// <summary>
    /// Gets or sets whether the item is equippable.
    /// </summary>
    public bool IsEquippable { get; init; }

    /// <summary>
    /// Gets or sets the item's icon name.
    /// </summary>
    public string? Icon { get; init; }

    /// <summary>
    /// Gets or sets the item's media URL.
    /// </summary>
    public string? MediaUrl { get; init; }

    /// <summary>
    /// Gets or sets the item's description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets or sets the maximum stack size.
    /// </summary>
    public int MaxStack { get; init; } = 1;

    /// <summary>
    /// Gets or sets the purchase price in copper.
    /// </summary>
    public long PurchasePrice { get; init; }

    /// <summary>
    /// Gets or sets the sell price in copper.
    /// </summary>
    public long SellPrice { get; init; }

    /// <summary>
    /// Gets or sets whether the item is soulbound.
    /// </summary>
    public bool IsSoulbound { get; init; }
}
