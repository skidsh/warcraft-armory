using WarcraftArmory.Domain.Entities;
using WarcraftArmory.Domain.Enums;

namespace WarcraftArmory.Application.Interfaces;

/// <summary>
/// Service interface for interacting with the Blizzard Game Data API.
/// </summary>
public interface IBlizzardApiService
{
    /// <summary>
    /// Gets a character by name, realm, and region.
    /// </summary>
    Task<Character?> GetCharacterAsync(string realm, string name, Region region, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an item by ID and region.
    /// </summary>
    Task<Item?> GetItemAsync(int itemId, Region region, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a guild by name, realm, and region.
    /// </summary>
    Task<Guild?> GetGuildAsync(string realm, string name, Region region, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a realm by slug and region.
    /// </summary>
    Task<Realm?> GetRealmAsync(string realmSlug, Region region, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all realms for a region.
    /// </summary>
    Task<IEnumerable<Realm>> GetRealmsAsync(Region region, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a mount by ID.
    /// </summary>
    Task<Mount?> GetMountAsync(int mountId, Region region, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all mounts for a region.
    /// </summary>
    Task<IEnumerable<Mount>> GetMountsAsync(Region region, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an achievement by ID.
    /// </summary>
    Task<Achievement?> GetAchievementAsync(int achievementId, Region region, CancellationToken cancellationToken = default);
}
