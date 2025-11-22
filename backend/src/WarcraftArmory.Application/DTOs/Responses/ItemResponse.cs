namespace WarcraftArmory.Application.DTOs.Responses;

/// <summary>
/// Response DTO for item data.
/// </summary>
public sealed record ItemResponse
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Quality { get; init; }
    public required int Level { get; init; }
    public int RequiredLevel { get; init; }
    public required int ItemClass { get; init; }
    public required int ItemSubclass { get; init; }
    public required int InventoryType { get; init; }
    public bool IsEquippable { get; init; }
    public string? Icon { get; init; }
    public string? MediaUrl { get; init; }
    public string? Description { get; init; }
    public int MaxStack { get; init; }
    public long PurchasePrice { get; init; }
    public long SellPrice { get; init; }
    public bool IsSoulbound { get; init; }
}
