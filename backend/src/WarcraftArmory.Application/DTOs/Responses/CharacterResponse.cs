using WarcraftArmory.Domain.Enums;

namespace WarcraftArmory.Application.DTOs.Responses;

/// <summary>
/// Response DTO for character data.
/// </summary>
public sealed record CharacterResponse
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Realm { get; init; }
    public required string Region { get; init; }
    public required string Class { get; init; }
    public required string Race { get; init; }
    public required int Level { get; init; }
    public required string Faction { get; init; }
    public required string Gender { get; init; }
    public int AchievementPoints { get; init; }
    public double AverageItemLevel { get; init; }
    public double EquippedItemLevel { get; init; }
    public string? ThumbnailUrl { get; init; }
    public int? GuildId { get; init; }
    public DateTime LastModified { get; init; }
}
