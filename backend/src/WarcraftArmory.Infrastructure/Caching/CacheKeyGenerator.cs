using System.Text;

namespace WarcraftArmory.Infrastructure.Caching;

/// <summary>
/// Generates consistent cache keys for Blizzard API data.
/// Pattern: wow:{region}:{namespace}:{category}:{id}:{version}
/// </summary>
public static class CacheKeyGenerator
{
    private const string Prefix = "wow";
    private const string Separator = ":";
    private const string Version = "v1";

    /// <summary>
    /// Generates a cache key for character data.
    /// Pattern: wow:us:profile:character:ragnaros:johndoe:v1
    /// </summary>
    /// <param name="region">Region code (us, eu, kr, tw, cn).</param>
    /// <param name="realmSlug">Realm slug (lowercase, hyphenated).</param>
    /// <param name="characterName">Character name (lowercase).</param>
    /// <returns>Cache key string.</returns>
    public static string ForCharacter(string region, string realmSlug, string characterName)
    {
        if (string.IsNullOrWhiteSpace(region))
            throw new ArgumentException("Region cannot be null or empty", nameof(region));
        if (string.IsNullOrWhiteSpace(realmSlug))
            throw new ArgumentException("Realm slug cannot be null or empty", nameof(realmSlug));
        if (string.IsNullOrWhiteSpace(characterName))
            throw new ArgumentException("Character name cannot be null or empty", nameof(characterName));

        return BuildKey(region, "profile", "character", $"{realmSlug}:{characterName}");
    }

    /// <summary>
    /// Generates a cache key for item data.
    /// Pattern: wow:us:static:item:18803:v1
    /// </summary>
    /// <param name="region">Region code.</param>
    /// <param name="itemId">Item ID.</param>
    /// <returns>Cache key string.</returns>
    public static string ForItem(string region, int itemId)
    {
        if (string.IsNullOrWhiteSpace(region))
            throw new ArgumentException("Region cannot be null or empty", nameof(region));
        if (itemId <= 0)
            throw new ArgumentException("Item ID must be greater than 0", nameof(itemId));

        return BuildKey(region, "static", "item", itemId.ToString());
    }

    /// <summary>
    /// Generates a cache key for guild data.
    /// Pattern: wow:us:profile:guild:ragnaros:guildname:v1
    /// </summary>
    /// <param name="region">Region code.</param>
    /// <param name="realmSlug">Realm slug.</param>
    /// <param name="guildName">Guild name (lowercase, url-encoded).</param>
    /// <returns>Cache key string.</returns>
    public static string ForGuild(string region, string realmSlug, string guildName)
    {
        if (string.IsNullOrWhiteSpace(region))
            throw new ArgumentException("Region cannot be null or empty", nameof(region));
        if (string.IsNullOrWhiteSpace(realmSlug))
            throw new ArgumentException("Realm slug cannot be null or empty", nameof(realmSlug));
        if (string.IsNullOrWhiteSpace(guildName))
            throw new ArgumentException("Guild name cannot be null or empty", nameof(guildName));

        return BuildKey(region, "profile", "guild", $"{realmSlug}:{guildName}");
    }

    /// <summary>
    /// Generates a cache key for realm data.
    /// Pattern: wow:us:dynamic:realm:ragnaros:v1
    /// </summary>
    /// <param name="region">Region code.</param>
    /// <param name="realmSlug">Realm slug.</param>
    /// <returns>Cache key string.</returns>
    public static string ForRealm(string region, string realmSlug)
    {
        if (string.IsNullOrWhiteSpace(region))
            throw new ArgumentException("Region cannot be null or empty", nameof(region));
        if (string.IsNullOrWhiteSpace(realmSlug))
            throw new ArgumentException("Realm slug cannot be null or empty", nameof(realmSlug));

        return BuildKey(region, "dynamic", "realm", realmSlug);
    }

    /// <summary>
    /// Generates a cache key for achievement data.
    /// Pattern: wow:us:static:achievement:123:v1
    /// </summary>
    /// <param name="region">Region code.</param>
    /// <param name="achievementId">Achievement ID.</param>
    /// <returns>Cache key string.</returns>
    public static string ForAchievement(string region, int achievementId)
    {
        if (string.IsNullOrWhiteSpace(region))
            throw new ArgumentException("Region cannot be null or empty", nameof(region));
        if (achievementId <= 0)
            throw new ArgumentException("Achievement ID must be greater than 0", nameof(achievementId));

        return BuildKey(region, "static", "achievement", achievementId.ToString());
    }

    /// <summary>
    /// Generates a cache key for mount data.
    /// Pattern: wow:us:static:mount:456:v1
    /// </summary>
    /// <param name="region">Region code.</param>
    /// <param name="mountId">Mount ID.</param>
    /// <returns>Cache key string.</returns>
    public static string ForMount(string region, int mountId)
    {
        if (string.IsNullOrWhiteSpace(region))
            throw new ArgumentException("Region cannot be null or empty", nameof(region));
        if (mountId <= 0)
            throw new ArgumentException("Mount ID must be greater than 0", nameof(mountId));

        return BuildKey(region, "static", "mount", mountId.ToString());
    }

    /// <summary>
    /// Generates a pattern for wildcard cache key matching.
    /// Pattern: wow:us:* (all US region data)
    /// </summary>
    /// <param name="region">Region code.</param>
    /// <param name="namespace">Optional namespace filter.</param>
    /// <param name="category">Optional category filter.</param>
    /// <returns>Cache key pattern.</returns>
    public static string Pattern(string region, string? @namespace = null, string? category = null)
    {
        if (string.IsNullOrWhiteSpace(region))
            throw new ArgumentException("Region cannot be null or empty", nameof(region));

        var parts = new List<string> { Prefix, region.ToLowerInvariant() };

        if (!string.IsNullOrWhiteSpace(@namespace))
        {
            parts.Add(@namespace.ToLowerInvariant());

            if (!string.IsNullOrWhiteSpace(category))
            {
                parts.Add(category.ToLowerInvariant());
            }
            else
            {
                parts.Add("*");
            }
        }
        else
        {
            parts.Add("*");
        }

        return string.Join(Separator, parts);
    }

    /// <summary>
    /// Builds a cache key from components.
    /// </summary>
    /// <param name="region">Region code.</param>
    /// <param name="namespace">Namespace (static, dynamic, profile).</param>
    /// <param name="category">Category (character, item, guild, etc.).</param>
    /// <param name="identifier">Unique identifier.</param>
    /// <returns">Formatted cache key.</returns>
    private static string BuildKey(string region, string @namespace, string category, string identifier)
    {
        var key = new StringBuilder()
            .Append(Prefix).Append(Separator)
            .Append(region.ToLowerInvariant()).Append(Separator)
            .Append(@namespace.ToLowerInvariant()).Append(Separator)
            .Append(category.ToLowerInvariant()).Append(Separator)
            .Append(identifier.ToLowerInvariant()).Append(Separator)
            .Append(Version)
            .ToString();

        return key;
    }

    /// <summary>
    /// Gets the namespace string for Blizzard API requests.
    /// </summary>
    /// <param name="namespaceType">Namespace type (static, dynamic, profile).</param>
    /// <param name="region">Region code.</param>
    /// <returns>Formatted namespace string (e.g., "static-us").</returns>
    public static string GetNamespace(string namespaceType, string region)
    {
        if (string.IsNullOrWhiteSpace(namespaceType))
            throw new ArgumentException("Namespace type cannot be null or empty", nameof(namespaceType));
        if (string.IsNullOrWhiteSpace(region))
            throw new ArgumentException("Region cannot be null or empty", nameof(region));

        return $"{namespaceType.ToLowerInvariant()}-{region.ToLowerInvariant()}";
    }
}
