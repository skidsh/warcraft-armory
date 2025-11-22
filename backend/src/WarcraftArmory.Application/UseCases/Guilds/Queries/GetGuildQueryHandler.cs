using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using WarcraftArmory.Application.DTOs.Responses;
using WarcraftArmory.Application.Interfaces;
using WarcraftArmory.Domain.Entities;

namespace WarcraftArmory.Application.UseCases.Guilds.Queries;

/// <summary>
/// Handler for GetGuildQuery.
/// </summary>
public sealed class GetGuildQueryHandler : IRequestHandler<GetGuildQuery, GuildResponse?>
{
    private readonly IBlizzardApiService _blizzardApiService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetGuildQueryHandler> _logger;

    private const int CacheDurationMinutes = 30;

    public GetGuildQueryHandler(
        IBlizzardApiService blizzardApiService,
        ICacheService cacheService,
        ILogger<GetGuildQueryHandler> logger)
    {
        _blizzardApiService = blizzardApiService;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<GuildResponse?> Handle(GetGuildQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"guild:{request.Region}:{request.Realm}:{request.Name}".ToLowerInvariant();

        _logger.LogInformation(
            "Fetching guild {GuildName} from realm {Realm} in region {Region}",
            request.Name, request.Realm, request.Region);

        // Try to get from cache first
        var cachedGuild = await _cacheService.GetAsync<Guild>(cacheKey, cancellationToken);
        if (cachedGuild != null)
        {
            return cachedGuild.Adapt<GuildResponse>();
        }

        // Fetch from API if not in cache
        var guild = await _blizzardApiService.GetGuildAsync(
            request.Realm, 
            request.Name, 
            request.Region, 
            cancellationToken);

        if (guild == null)
        {
            _logger.LogWarning(
                "Guild {GuildName} not found on realm {Realm} in region {Region}",
                request.Name, request.Realm, request.Region);
            return null;
        }

        // Cache the result
        await _cacheService.SetAsync(
            cacheKey, 
            guild, 
            TimeSpan.FromMinutes(CacheDurationMinutes), 
            cancellationToken);

        return guild.Adapt<GuildResponse>();
    }
}
