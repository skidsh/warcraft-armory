using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using WarcraftArmory.Application.Interfaces;

namespace WarcraftArmory.Infrastructure.Caching;

/// <summary>
/// Redis-based distributed cache service implementation.
/// Provides thread-safe caching with TTL support across multiple application instances.
/// </summary>
public sealed class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(
        IConnectionMultiplexer redis,
        ILogger<RedisCacheService> logger)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Cache key cannot be null or empty", nameof(key));

        try
        {
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync(key);

            if (!value.HasValue || value.IsNullOrEmpty)
            {
                _logger.LogDebug("Cache miss for key: {Key}", key);
                return null;
            }

            var deserializedValue = JsonSerializer.Deserialize<T>(value.ToString(), _jsonOptions);
            _logger.LogDebug("Cache hit for key: {Key}", key);
            return deserializedValue;
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Redis error while getting key: {Key}", key);
            // Return null on cache errors to allow fallback to source
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Deserialization error for key: {Key}", key);
            // Remove corrupted cache entry
            await RemoveAsync(key, cancellationToken);
            return null;
        }
    }

    public async Task SetAsync<T>(
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

        try
        {
            var db = _redis.GetDatabase();
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            
            await db.StringSetAsync(key, serializedValue, expiration);
            
            _logger.LogDebug("Cached value for key: {Key} with expiration: {Expiration}", key, expiration);
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Redis error while setting key: {Key}", key);
            // Don't throw - caching is not critical
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Serialization error for key: {Key}", key);
            // Don't throw - caching is not critical
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Cache key cannot be null or empty", nameof(key));

        try
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(key);
            
            _logger.LogDebug("Removed cache key: {Key}", key);
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Redis error while removing key: {Key}", key);
            // Don't throw - removal failure is not critical
        }
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
        var cachedValue = await GetAsync<T>(key, cancellationToken);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        // Cache miss - execute factory to get value
        _logger.LogDebug("Cache miss for key: {Key}, executing factory", key);
        var value = await factory();

        // Store in cache
        if (value != null)
        {
            await SetAsync(key, value, expiration, cancellationToken);
            return value;
        }

        throw new InvalidOperationException($"Factory returned null for cache key: {key}");
    }

    /// <summary>
    /// Removes all keys matching a pattern.
    /// WARNING: Use with caution in production - can be expensive.
    /// </summary>
    /// <param name="pattern">Key pattern (e.g., "wow:us:*").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            throw new ArgumentException("Pattern cannot be null or empty", nameof(pattern));

        try
        {
            var endpoints = _redis.GetEndPoints();
            foreach (var endpoint in endpoints)
            {
                var server = _redis.GetServer(endpoint);
                var keys = server.Keys(pattern: pattern).ToArray();
                
                if (keys.Length > 0)
                {
                    var db = _redis.GetDatabase();
                    await db.KeyDeleteAsync(keys);
                    
                    _logger.LogInformation("Removed {Count} keys matching pattern: {Pattern}", 
                        keys.Length, pattern);
                }
            }
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Redis error while removing keys by pattern: {Pattern}", pattern);
        }
    }

    /// <summary>
    /// Checks if a key exists in the cache.
    /// </summary>
    /// <param name="key">Cache key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if key exists, otherwise false.</returns>
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Cache key cannot be null or empty", nameof(key));

        try
        {
            var db = _redis.GetDatabase();
            return await db.KeyExistsAsync(key);
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Redis error while checking key existence: {Key}", key);
            return false;
        }
    }
}
