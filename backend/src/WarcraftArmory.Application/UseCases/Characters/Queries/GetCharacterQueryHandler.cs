using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using WarcraftArmory.Application.DTOs.Responses;
using WarcraftArmory.Application.Interfaces;
using WarcraftArmory.Domain.Entities;

namespace WarcraftArmory.Application.UseCases.Characters.Queries;

/// <summary>
/// Handler for GetCharacterQuery.
/// </summary>
public sealed class GetCharacterQueryHandler : IRequestHandler<GetCharacterQuery, CharacterResponse?>
{
    private readonly IBlizzardApiService _blizzardApiService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetCharacterQueryHandler> _logger;

    private const int CacheDurationMinutes = 15;

    public GetCharacterQueryHandler(
        IBlizzardApiService blizzardApiService,
        ICacheService cacheService,
        ILogger<GetCharacterQueryHandler> logger)
    {
        _blizzardApiService = blizzardApiService;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<CharacterResponse?> Handle(GetCharacterQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"character:{request.Region}:{request.Realm}:{request.Name}".ToLowerInvariant();

        _logger.LogInformation(
            "Fetching character {CharacterName} from realm {Realm} in region {Region}",
            request.Name, request.Realm, request.Region);

        // Try to get from cache first
        var cachedCharacter = await _cacheService.GetAsync<Character>(cacheKey, cancellationToken);
        if (cachedCharacter != null)
        {
            return cachedCharacter.Adapt<CharacterResponse>();
        }

        // Fetch from API if not in cache
        var character = await _blizzardApiService.GetCharacterAsync(
            request.Realm, 
            request.Name, 
            request.Region, 
            cancellationToken);

        if (character == null)
        {
            _logger.LogWarning(
                "Character {CharacterName} not found on realm {Realm} in region {Region}",
                request.Name, request.Realm, request.Region);
            return null;
        }

        // Cache the result
        await _cacheService.SetAsync(
            cacheKey, 
            character, 
            TimeSpan.FromMinutes(CacheDurationMinutes), 
            cancellationToken);

        return character.Adapt<CharacterResponse>();
    }
}
