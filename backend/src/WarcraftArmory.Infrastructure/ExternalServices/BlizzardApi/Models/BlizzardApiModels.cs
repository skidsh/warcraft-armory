using System.Text.Json.Serialization;

namespace WarcraftArmory.Infrastructure.ExternalServices.BlizzardApi.Models;

/// <summary>
/// Blizzard API response model for character data.
/// Maps to the character profile endpoint response.
/// </summary>
public sealed record BlizzardCharacter
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("gender")]
    public required BlizzardGenderType Gender { get; init; }

    [JsonPropertyName("faction")]
    public required BlizzardFactionType Faction { get; init; }

    [JsonPropertyName("race")]
    public BlizzardReference? Race { get; init; }

    [JsonPropertyName("character_class")]
    public BlizzardReference? CharacterClass { get; init; }

    [JsonPropertyName("active_spec")]
    public BlizzardReference? ActiveSpec { get; init; }

    [JsonPropertyName("realm")]
    public BlizzardRealm? Realm { get; init; }

    [JsonPropertyName("guild")]
    public BlizzardGuildReference? Guild { get; init; }

    [JsonPropertyName("level")]
    public int Level { get; init; }

    [JsonPropertyName("experience")]
    public int Experience { get; init; }

    [JsonPropertyName("achievement_points")]
    public int AchievementPoints { get; init; }

    [JsonPropertyName("last_login_timestamp")]
    public long LastLoginTimestamp { get; init; }

    [JsonPropertyName("average_item_level")]
    public int AverageItemLevel { get; init; }

    [JsonPropertyName("equipped_item_level")]
    public int EquippedItemLevel { get; init; }
}

/// <summary>
/// Blizzard API response model for item data.
/// </summary>
public sealed record BlizzardItem
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("quality")]
    public required BlizzardQualityType Quality { get; init; }

    [JsonPropertyName("level")]
    public int Level { get; init; }

    [JsonPropertyName("required_level")]
    public int RequiredLevel { get; init; }

    [JsonPropertyName("media")]
    public BlizzardAsset? Media { get; init; }

    [JsonPropertyName("item_class")]
    public BlizzardReference? ItemClass { get; init; }

    [JsonPropertyName("item_subclass")]
    public BlizzardReference? ItemSubclass { get; init; }

    [JsonPropertyName("inventory_type")]
    public BlizzardInventoryType? InventoryType { get; init; }

    [JsonPropertyName("purchase_price")]
    public long PurchasePrice { get; init; }

    [JsonPropertyName("sell_price")]
    public long SellPrice { get; init; }

    [JsonPropertyName("max_count")]
    public int MaxCount { get; init; }

    [JsonPropertyName("is_equippable")]
    public bool IsEquippable { get; init; }

    [JsonPropertyName("is_stackable")]
    public bool IsStackable { get; init; }
}

/// <summary>
/// Blizzard API response model for guild data.
/// </summary>
public sealed record BlizzardGuild
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("faction")]
    public required BlizzardFactionType Faction { get; init; }

    [JsonPropertyName("achievement_points")]
    public int AchievementPoints { get; init; }

    [JsonPropertyName("member_count")]
    public int MemberCount { get; init; }

    [JsonPropertyName("created_timestamp")]
    public long CreatedTimestamp { get; init; }

    [JsonPropertyName("realm")]
    public BlizzardRealm? Realm { get; init; }
}

/// <summary>
/// Generic reference to another Blizzard API resource.
/// </summary>
public sealed record BlizzardReference
{
    [JsonPropertyName("key")]
    public BlizzardLink? Key { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("id")]
    public int Id { get; init; }
}

/// <summary>
/// Realm information from Blizzard API.
/// </summary>
public sealed record BlizzardRealm
{
    [JsonPropertyName("key")]
    public BlizzardLink? Key { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("slug")]
    public required string Slug { get; init; }
}

/// <summary>
/// Guild reference (minimal guild info).
/// </summary>
public sealed record BlizzardGuildReference
{
    [JsonPropertyName("key")]
    public BlizzardLink? Key { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("realm")]
    public BlizzardRealm? Realm { get; init; }

    [JsonPropertyName("faction")]
    public required BlizzardFactionType Faction { get; init; }
}

/// <summary>
/// API link/href reference.
/// </summary>
public sealed record BlizzardLink
{
    [JsonPropertyName("href")]
    public required string Href { get; init; }
}

/// <summary>
/// Media/asset reference (icons, renders).
/// </summary>
public sealed record BlizzardAsset
{
    [JsonPropertyName("key")]
    public BlizzardLink? Key { get; init; }

    [JsonPropertyName("id")]
    public int Id { get; init; }
}

/// <summary>
/// Inventory type information.
/// </summary>
public sealed record BlizzardInventoryType
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }
}

// Enumerations matching Blizzard API values

public sealed record BlizzardGenderType
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }
}

public sealed record BlizzardFactionType
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }
}

public sealed record BlizzardQualityType
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }
}
