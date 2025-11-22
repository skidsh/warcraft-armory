using Refit;
using WarcraftArmory.Infrastructure.ExternalServices.BlizzardApi.Models;

namespace WarcraftArmory.Infrastructure.ExternalServices.BlizzardApi;

/// <summary>
/// Refit interface for the Blizzard Game Data API.
/// Defines typed HTTP endpoints for character, item, and guild data retrieval.
/// </summary>
public interface IBlizzardApiClient
{
    /// <summary>
    /// Gets character profile data by realm and name.
    /// </summary>
    /// <param name="realmSlug">Realm slug (lowercase, hyphenated).</param>
    /// <param name="characterName">Character name (lowercase).</param>
    /// <param name="namespace">Namespace (profile-{region}).</param>
    /// <param name="locale">Locale code (en_US, es_MX, etc.).</param>
    /// <returns>Character profile data.</returns>
    [Get("/profile/wow/character/{realmSlug}/{characterName}")]
    Task<BlizzardCharacter> GetCharacterAsync(
        [AliasAs("realmSlug")] string realmSlug,
        [AliasAs("characterName")] string characterName,
        [Query] string @namespace,
        [Query] string locale = "en_US");

    /// <summary>
    /// Gets item data by item ID.
    /// </summary>
    /// <param name="itemId">Item ID.</param>
    /// <param name="namespace">Namespace (static-{region}).</param>
    /// <param name="locale">Locale code.</param>
    /// <returns>Item data.</returns>
    [Get("/data/wow/item/{itemId}")]
    Task<BlizzardItem> GetItemAsync(
        [AliasAs("itemId")] int itemId,
        [Query] string @namespace,
        [Query] string locale = "en_US");

    /// <summary>
    /// Gets guild profile data by realm and name.
    /// </summary>
    /// <param name="realmSlug">Realm slug (lowercase, hyphenated).</param>
    /// <param name="guildName">Guild name (lowercase, url-encoded).</param>
    /// <param name="namespace">Namespace (profile-{region}).</param>
    /// <param name="locale">Locale code.</param>
    /// <returns">Guild profile data.</returns>
    [Get("/data/wow/guild/{realmSlug}/{guildName}")]
    Task<BlizzardGuild> GetGuildAsync(
        [AliasAs("realmSlug")] string realmSlug,
        [AliasAs("guildName")] string guildName,
        [Query] string @namespace,
        [Query] string locale = "en_US");

    /// <summary>
    /// Gets realm data by realm slug.
    /// </summary>
    /// <param name="realmSlug">Realm slug.</param>
    /// <param name="namespace">Namespace (dynamic-{region}).</param>
    /// <param name="locale">Locale code.</param>
    /// <returns>Realm data.</returns>
    [Get("/data/wow/realm/{realmSlug}")]
    Task<BlizzardRealm> GetRealmAsync(
        [AliasAs("realmSlug")] string realmSlug,
        [Query] string @namespace,
        [Query] string locale = "en_US");
}
