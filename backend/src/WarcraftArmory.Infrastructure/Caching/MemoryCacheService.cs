using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using WarcraftArmory.Application.Interfaces;

namespace WarcraftArmory.Infrastructure.Caching;

/// <summary>
/// In-memory cache service implementation using IMemoryCache.
/// Provides fast, process-local caching for hot data and OAuth tokens.
/// </summary>
public sealed class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryCacheService> _logger;

    public MemoryCacheService(
        IMemoryCache memoryCache,
        ILogger<MemoryCacheService> logger)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Cache key cannot be null or empty", nameof(key));

        if (_memoryCache.TryGetValue<T>(key, out var value))
        {
            _logger.LogDebug("Memory cache hit for key: {Key}", key);
            return Task.FromResult<T?>(value);
        }

        _logger.LogDebug("Memory cache miss for key: {Key}", key);
        return Task.FromResult<T?>(null);
    }

    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken = default) where T : class
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Cache key cannot be null or empty", nameof(key));

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (expiration <= TimeSpan.Zero)
            throw new ArgumentException("Expiration must be greater than zero", nameof(expiration));

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(expiration)
            .SetPriority(CacheItemPriority.Normal);

        _memoryCache.Set(key, value, cacheEntryOptions);

        _logger.LogDebug("Cached value in memory for key: {Key} with expiration: {Expiration}", 
            key, expiration);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Cache key cannot be null or empty", nameof(key));

        _memoryCache.Remove(key);
        
        _logger.LogDebug("Removed memory cache key: {Key}", key);
        
        return Task.CompletedTask;
    }

    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan expiration,
        CancellationToken cancellationToken = default) where T : class
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Cache key cannot be null or empty", nameof(key));

        if (factory == null)
            throw new ArgumentNullException(nameof(factory));

        // Try to get from cache first
        if (_memoryCache.TryGetValue<T>(key, out var cachedValue) && cachedValue != null)
        {
            _logger.LogDebug("Memory cache hit for key: {Key}", key);
            return cachedValue;
        }

        // Cache miss - execute factory to get value
        _logger.LogDebug("Memory cache miss for key: {Key}, executing factory", key);
        var value = await factory();

        // Store in cache
        if (value != null)
        {
            await SetAsync(key, value, expiration, cancellationToken);
            return value;
        }

        throw new InvalidOperationException($"Factory returned null for cache key: {key}");
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Cache key cannot be null or empty", nameof(key));

        var exists = _memoryCache.TryGetValue(key, out _);
        return Task.FromResult(exists);
    }

    /// <summary>
    /// Clears all items from the memory cache.
    /// WARNING: Use with caution - this affects all cached items.
    /// </summary>
    public void Clear()
    {
        if (_memoryCache is MemoryCache concreteCache)
        {
            concreteCache.Compact(1.0); // Remove 100% of items
            _logger.LogWarning("Memory cache cleared (compacted 100%)");
        }
        else
        {
            _logger.LogWarning("Cannot clear memory cache - compact operation not supported");
        }
    }
}
