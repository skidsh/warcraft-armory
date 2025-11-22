using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using WarcraftArmory.Application.DTOs.Responses;
using WarcraftArmory.Application.Interfaces;
using WarcraftArmory.Domain.Entities;

namespace WarcraftArmory.Application.UseCases.Items.Queries;

/// <summary>
/// Handler for GetItemQuery.
/// </summary>
public sealed class GetItemQueryHandler : IRequestHandler<GetItemQuery, ItemResponse?>
{
    private readonly IBlizzardApiService _blizzardApiService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetItemQueryHandler> _logger;

    private const int CacheDurationHours = 24; // Items are static data

    public GetItemQueryHandler(
        IBlizzardApiService blizzardApiService,
        ICacheService cacheService,
        ILogger<GetItemQueryHandler> logger)
    {
        _blizzardApiService = blizzardApiService;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ItemResponse?> Handle(GetItemQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"item:{request.Region}:{request.ItemId}";

        _logger.LogInformation(
            "Fetching item {ItemId} from region {Region}",
            request.ItemId, request.Region);

        // Try to get from cache first
        var cachedItem = await _cacheService.GetAsync<Item>(cacheKey, cancellationToken);
        if (cachedItem != null)
        {
            return cachedItem.Adapt<ItemResponse>();
        }

        // Fetch from API if not in cache
        var item = await _blizzardApiService.GetItemAsync(
            request.ItemId, 
            request.Region, 
            cancellationToken);

        if (item == null)
        {
            _logger.LogWarning(
                "Item {ItemId} not found in region {Region}",
                request.ItemId, request.Region);
            return null;
        }

        // Cache the result
        await _cacheService.SetAsync(
            cacheKey, 
            item, 
            TimeSpan.FromHours(CacheDurationHours), 
            cancellationToken);

        return item.Adapt<ItemResponse>();
    }
}
