using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WarcraftArmory.Application.Interfaces;
using WarcraftArmory.Domain.Entities;
using WarcraftArmory.Domain.Enums;
using WarcraftArmory.Infrastructure.Caching;
using WarcraftArmory.Infrastructure.ExternalServices.BlizzardApi;
using WarcraftArmory.Infrastructure.ExternalServices.Configuration;

namespace WarcraftArmory.Infrastructure.Services;

/// <summary>
/// Service for interacting with the Blizzard Game Data API.
/// Implements caching, distributed rate limiting, and entity mapping.
/// </summary>
public sealed class BlizzardApiService : IBlizzardApiService
{
    private readonly IBlizzardApiClient _apiClient;
    private readonly ICacheService _cacheService;
    private readonly DistributedRateLimiter _rateLimiter;
    private readonly ILogger<BlizzardApiService> _logger;
    private readonly BlizzardApiSettings _settings;

    public BlizzardApiService(
        IBlizzardApiClient apiClient,
        ICacheService cacheService,
        DistributedRateLimiter rateLimiter,
        ILogger<BlizzardApiService> logger,
        IOptions<BlizzardApiSettings> settings)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _rateLimiter = rateLimiter ?? throw new ArgumentNullException(nameof(rateLimiter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
    }

    public async Task<Character?> GetCharacterAsync(
        string realm,
        string name,
        Region region,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(realm))
            throw new ArgumentException("Realm cannot be null or empty", nameof(realm));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        var regionString = region.ToString().ToLowerInvariant();
        var realmSlug = realm.ToLowerInvariant().Replace(" ", "-");
        var characterName = name.ToLowerInvariant();
        var cacheKey = CacheKeyGenerator.ForCharacter(regionString, realmSlug, characterName);

        _logger.LogInformation(
            "Fetching character: {CharacterName} from realm: {Realm} in region: {Region}",
            characterName, realmSlug, regionString);

        return await _cacheService.GetOrSetAsync(
            cacheKey,
            async () =>
            {
                // Wait for distributed rate limiter
                await _rateLimiter.WaitForBlizzardApiSlotAsync(cancellationToken);

                // Call Blizzard API
                var @namespace = CacheKeyGenerator.GetNamespace("profile", regionString);
                var blizzardCharacter = await _apiClient.GetCharacterAsync(
                    regionString, realmSlug, characterName, @namespace);

                // Map to domain entity
                return MapToDomainCharacter(blizzardCharacter, realm, region);
            },
            TimeSpan.FromMinutes(30), // Profile data: 30 minutes TTL
            cancellationToken);
    }

    public async Task<Item?> GetItemAsync(
        int itemId,
        Region region,
        CancellationToken cancellationToken = default)
    {
        if (itemId <= 0)
            throw new ArgumentException("Item ID must be greater than 0", nameof(itemId));

        var regionString = region.ToString().ToLowerInvariant();
        var cacheKey = CacheKeyGenerator.ForItem(regionString, itemId);

        _logger.LogInformation("Fetching item: {ItemId} in region: {Region}", itemId, regionString);

        return await _cacheService.GetOrSetAsync(
            cacheKey,
            async () =>
            {
                // Wait for distributed rate limiter
                await _rateLimiter.WaitForBlizzardApiSlotAsync(cancellationToken);

                // Call Blizzard API
                var @namespace = CacheKeyGenerator.GetNamespace("static", regionString);
                var blizzardItem = await _apiClient.GetItemAsync(
                    regionString, itemId, @namespace);

                // Map to domain entity
                return MapToDomainItem(blizzardItem);
            },
            TimeSpan.FromDays(7), // Static item data: 7 days TTL
            cancellationToken);
    }

    public async Task<Guild?> GetGuildAsync(
        string realm,
        string name,
        Region region,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(realm))
            throw new ArgumentException("Realm cannot be null or empty", nameof(realm));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        var regionString = region.ToString().ToLowerInvariant();
        var realmSlug = realm.ToLowerInvariant().Replace(" ", "-");
        var guildName = Uri.EscapeDataString(name.ToLowerInvariant());
        var cacheKey = CacheKeyGenerator.ForGuild(regionString, realmSlug, guildName);

        _logger.LogInformation(
            "Fetching guild: {GuildName} from realm: {Realm} in region: {Region}",
            guildName, realmSlug, regionString);

        return await _cacheService.GetOrSetAsync(
            cacheKey,
            async () =>
            {
                // Wait for distributed rate limiter
                await _rateLimiter.WaitForBlizzardApiSlotAsync(cancellationToken);

                // Call Blizzard API
                var @namespace = CacheKeyGenerator.GetNamespace("profile", regionString);
                var blizzardGuild = await _apiClient.GetGuildAsync(
                    regionString, realmSlug, guildName, @namespace);

                // Map to domain entity
                return MapToDomainGuild(blizzardGuild, realm, region);
            },
            TimeSpan.FromHours(1), // Guild data: 1 hour TTL
            cancellationToken);
    }

    public async Task<Realm?> GetRealmAsync(
        string realmSlug,
        Region region,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(realmSlug))
            throw new ArgumentException("Realm slug cannot be null or empty", nameof(realmSlug));

        var regionString = region.ToString().ToLowerInvariant();
        var normalizedSlug = realmSlug.ToLowerInvariant().Replace(" ", "-");
        var cacheKey = CacheKeyGenerator.ForRealm(regionString, normalizedSlug);

        _logger.LogInformation("Fetching realm: {RealmSlug} in region: {Region}", normalizedSlug, regionString);

        return await _cacheService.GetOrSetAsync(
            cacheKey,
            async () =>
            {
                // Wait for distributed rate limiter
                await _rateLimiter.WaitForBlizzardApiSlotAsync(cancellationToken);

                // Call Blizzard API
                var @namespace = CacheKeyGenerator.GetNamespace("dynamic", regionString);
                var blizzardRealm = await _apiClient.GetRealmAsync(
                    regionString, normalizedSlug, @namespace);

                // Map to domain entity
                return MapToDomainRealm(blizzardRealm, region);
            },
            TimeSpan.FromMinutes(10), // Realm data: 10 minutes TTL
            cancellationToken);
    }

    public Task<IEnumerable<Realm>> GetRealmsAsync(
        Region region,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement when Blizzard API realms index endpoint is added to IBlizzardApiClient
        _logger.LogWarning("GetRealmsAsync not yet implemented - requires realms index endpoint");
        throw new NotImplementedException("GetRealmsAsync requires implementation of realms index API endpoint");
    }

    public Task<Mount?> GetMountAsync(
        int mountId,
        Region region,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement when Blizzard API mount endpoint is added to IBlizzardApiClient
        _logger.LogWarning("GetMountAsync not yet implemented - requires mount endpoint");
        throw new NotImplementedException("GetMountAsync requires implementation of mount API endpoint");
    }

    public Task<IEnumerable<Mount>> GetMountsAsync(
        Region region,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement when Blizzard API mounts index endpoint is added to IBlizzardApiClient
        _logger.LogWarning("GetMountsAsync not yet implemented - requires mounts index endpoint");
        throw new NotImplementedException("GetMountsAsync requires implementation of mounts index API endpoint");
    }

    public Task<Achievement?> GetAchievementAsync(
        int achievementId,
        Region region,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement when Blizzard API achievement endpoint is added to IBlizzardApiClient
        _logger.LogWarning("GetAchievementAsync not yet implemented - requires achievement endpoint");
        throw new NotImplementedException("GetAchievementAsync requires implementation of achievement API endpoint");
    }

    // Private mapping methods - Convert Blizzard API models to Domain entities

    private static Character MapToDomainCharacter(
        ExternalServices.BlizzardApi.Models.BlizzardCharacter blizzardCharacter,
        string realm,
        Region region)
    {
        return new Character
        {
            Id = (int)blizzardCharacter.Id,
            Name = blizzardCharacter.Name,
            Realm = realm,
            Region = region,
            Level = blizzardCharacter.Level,
            Class = MapClass(blizzardCharacter.CharacterClass?.Name),
            Race = MapRace(blizzardCharacter.Race?.Name),
            Gender = MapGender(blizzardCharacter.Gender.Type),
            Faction = MapFaction(blizzardCharacter.Faction.Type),
            GuildId = blizzardCharacter.Guild != null ? (int?)blizzardCharacter.Guild.Id : null,
            AchievementPoints = blizzardCharacter.AchievementPoints,
            AverageItemLevel = blizzardCharacter.AverageItemLevel,
            EquippedItemLevel = blizzardCharacter.EquippedItemLevel,
            LastModified = DateTime.UtcNow
        };
    }

    private static Item MapToDomainItem(
        ExternalServices.BlizzardApi.Models.BlizzardItem blizzardItem)
    {
        return new Item
        {
            Id = blizzardItem.Id,
            Name = blizzardItem.Name,
            Quality = MapQuality(blizzardItem.Quality.Type),
            Level = blizzardItem.Level,
            RequiredLevel = blizzardItem.RequiredLevel,
            ItemClass = blizzardItem.ItemClass?.Id ?? 0,
            ItemSubclass = blizzardItem.ItemSubclass?.Id ?? 0,
            InventoryType = 0, // TODO: Map from blizzardItem.InventoryType when available
            MaxStack = blizzardItem.MaxCount,
            IsEquippable = blizzardItem.IsEquippable,
            PurchasePrice = blizzardItem.PurchasePrice,
            SellPrice = blizzardItem.SellPrice
        };
    }

    private static Guild MapToDomainGuild(
        ExternalServices.BlizzardApi.Models.BlizzardGuild blizzardGuild,
        string realm,
        Region region)
    {
        return new Guild
        {
            Id = (int)blizzardGuild.Id,
            Name = blizzardGuild.Name,
            Realm = realm,
            Region = region,
            Faction = MapFaction(blizzardGuild.Faction.Type),
            MemberCount = blizzardGuild.MemberCount,
            AchievementPoints = blizzardGuild.AchievementPoints,
            CreatedTimestamp = DateTimeOffset.FromUnixTimeMilliseconds(blizzardGuild.CreatedTimestamp).DateTime,
            LastModified = DateTime.UtcNow
        };
    }

    private static Realm MapToDomainRealm(
        ExternalServices.BlizzardApi.Models.BlizzardRealm blizzardRealm,
        Region region)
    {
        return new Realm
        {
            Id = blizzardRealm.Id,
            Name = blizzardRealm.Name,
            Slug = blizzardRealm.Slug,
            Region = region,
            Locale = "en_US", // TODO: Determine from region or API response
            Timezone = GetTimezoneForRegion(region),
            Category = region.ToString().ToLowerInvariant(),
            IsTournament = false // TODO: Determine from API response if available
        };
    }

    private static string GetTimezoneForRegion(Region region)
    {
        return region switch
        {
            Region.US => "America/Los_Angeles",
            Region.EU => "Europe/Paris",
            Region.KR => "Asia/Seoul",
            Region.TW => "Asia/Taipei",
            Region.CN => "Asia/Shanghai",
            _ => "UTC"
        };
    }

    // Enum mapping helpers - Convert Blizzard API strings to Domain enums

    private static CharacterClass MapClass(string? className)
    {
        return className?.ToLowerInvariant() switch
        {
            "warrior" => CharacterClass.Warrior,
            "paladin" => CharacterClass.Paladin,
            "hunter" => CharacterClass.Hunter,
            "rogue" => CharacterClass.Rogue,
            "priest" => CharacterClass.Priest,
            "death knight" => CharacterClass.DeathKnight,
            "shaman" => CharacterClass.Shaman,
            "mage" => CharacterClass.Mage,
            "warlock" => CharacterClass.Warlock,
            "monk" => CharacterClass.Monk,
            "druid" => CharacterClass.Druid,
            "demon hunter" => CharacterClass.DemonHunter,
            "evoker" => CharacterClass.Evoker,
            _ => CharacterClass.Warrior // Default fallback
        };
    }

    private static CharacterRace MapRace(string? raceName)
    {
        return raceName?.ToLowerInvariant() switch
        {
            "human" => CharacterRace.Human,
            "orc" => CharacterRace.Orc,
            "dwarf" => CharacterRace.Dwarf,
            "night elf" => CharacterRace.NightElf,
            "undead" => CharacterRace.Undead,
            "tauren" => CharacterRace.Tauren,
            "gnome" => CharacterRace.Gnome,
            "troll" => CharacterRace.Troll,
            "blood elf" => CharacterRace.BloodElf,
            "draenei" => CharacterRace.Draenei,
            _ => CharacterRace.Human // Default fallback
        };
    }

    private static Gender MapGender(string genderType)
    {
        return genderType.ToLowerInvariant() switch
        {
            "male" => Gender.Male,
            "female" => Gender.Female,
            _ => Gender.Male // Default fallback
        };
    }

    private static Faction MapFaction(string factionType)
    {
        return factionType.ToLowerInvariant() switch
        {
            "alliance" => Faction.Alliance,
            "horde" => Faction.Horde,
            _ => Faction.Alliance // Default fallback
        };
    }

    private static ItemQuality MapQuality(string qualityType)
    {
        return qualityType.ToLowerInvariant() switch
        {
            "poor" => ItemQuality.Poor,
            "common" => ItemQuality.Common,
            "uncommon" => ItemQuality.Uncommon,
            "rare" => ItemQuality.Rare,
            "epic" => ItemQuality.Epic,
            "legendary" => ItemQuality.Legendary,
            "artifact" => ItemQuality.Artifact,
            "heirloom" => ItemQuality.Heirloom,
            _ => ItemQuality.Common // Default fallback
        };
    }
}
