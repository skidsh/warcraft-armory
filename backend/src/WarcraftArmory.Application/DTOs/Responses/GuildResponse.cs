namespace WarcraftArmory.Application.DTOs.Responses;

/// <summary>
/// Response DTO for guild data.
/// </summary>
public sealed record GuildResponse
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Realm { get; init; }
    public required string Region { get; init; }
    public required string Faction { get; init; }
    public int AchievementPoints { get; init; }
    public int MemberCount { get; init; }
    public string? EmblemData { get; init; }
    public DateTime CreatedTimestamp { get; init; }
    public DateTime LastModified { get; init; }
}
