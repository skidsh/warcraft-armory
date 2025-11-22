using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using WarcraftArmory.Infrastructure.ExternalServices.Configuration;

namespace WarcraftArmory.Infrastructure.ExternalServices.BlizzardApi;

/// <summary>
/// Distributed rate limiter using Redis for tracking limits across multiple application instances.
/// Implements both global Blizzard API rate limiting and per-user rate limiting.
/// Thread-safe and works across multiple pods/containers.
/// </summary>
public sealed class DistributedRateLimiter : IDisposable
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<DistributedRateLimiter> _logger;
    private readonly BlizzardApiSettings _settings;
    
    // Redis keys
    private const string BlizzardRateLimitKeyPrefix = "ratelimit:blizzard";
    private const string UserRateLimitKeyPrefix = "ratelimit:user";
    
    // Conservative limits - we want to stay well below Blizzard's actual limits
    // Blizzard allows 100 req/sec and 36,000 req/hour
    // We'll target 80% of that to be safe: 80 req/sec, 28,800 req/hour
    private const int GlobalPerSecondLimit = 80;
    private const int GlobalPerHourLimit = 28800;
    
    // Per-user limits to prevent abuse
    private const int UserPerMinuteLimit = 60;  // 1 request per second per user
    private const int UserPerHourLimit = 1000;   // ~16 requests per minute per user
    
    private bool _disposed;

    public DistributedRateLimiter(
        IConnectionMultiplexer redis,
        ILogger<DistributedRateLimiter> logger,
        IOptions<BlizzardApiSettings> settings)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
    }

    /// <summary>
    /// Checks if a user request is allowed based on per-user rate limits.
    /// This should be called by API controllers to limit user requests.
    /// </summary>
    /// <param name="userId">User identifier (IP address, user ID, or API key).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if request is allowed, false if rate limit exceeded.</returns>
    public async Task<bool> IsUserRequestAllowedAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be null or empty", nameof(userId));

        try
        {
            var db = _redis.GetDatabase();
            var now = DateTimeOffset.UtcNow;
            
            // Check per-minute limit
            var minuteKey = $"{UserRateLimitKeyPrefix}:{userId}:minute:{now:yyyyMMddHHmm}";
            var minuteCount = await db.StringIncrementAsync(minuteKey);
            
            if (minuteCount == 1)
            {
                // First request in this minute - set expiration
                await db.KeyExpireAsync(minuteKey, TimeSpan.FromMinutes(2));
            }
            
            if (minuteCount > UserPerMinuteLimit)
            {
                _logger.LogWarning(
                    "User {UserId} exceeded per-minute rate limit ({Count}/{Limit})",
                    userId, minuteCount, UserPerMinuteLimit);
                return false;
            }
            
            // Check per-hour limit
            var hourKey = $"{UserRateLimitKeyPrefix}:{userId}:hour:{now:yyyyMMddHH}";
            var hourCount = await db.StringIncrementAsync(hourKey);
            
            if (hourCount == 1)
            {
                // First request in this hour - set expiration
                await db.KeyExpireAsync(hourKey, TimeSpan.FromHours(2));
            }
            
            if (hourCount > UserPerHourLimit)
            {
                _logger.LogWarning(
                    "User {UserId} exceeded per-hour rate limit ({Count}/{Limit})",
                    userId, hourCount, UserPerHourLimit);
                return false;
            }
            
            return true;
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Redis error while checking user rate limit for {UserId}", userId);
            // On Redis errors, allow the request (fail open)
            return true;
        }
    }

    /// <summary>
    /// Waits until a request to Blizzard API is allowed based on global rate limits.
    /// This ensures all pods combined don't exceed Blizzard's rate limits.
    /// Should be called before making Blizzard API requests.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task that completes when request is allowed.</returns>
    public async Task WaitForBlizzardApiSlotAsync(CancellationToken cancellationToken = default)
    {
        var maxRetries = 10;
        var retryCount = 0;
        
        while (retryCount < maxRetries)
        {
            try
            {
                var db = _redis.GetDatabase();
                var now = DateTimeOffset.UtcNow;
                
                // Check per-second limit using sliding window
                var secondKey = $"{BlizzardRateLimitKeyPrefix}:second:{now:yyyyMMddHHmmss}";
                var secondCount = await db.StringIncrementAsync(secondKey);
                
                if (secondCount == 1)
                {
                    // First request in this second - set expiration
                    await db.KeyExpireAsync(secondKey, TimeSpan.FromSeconds(5));
                }
                
                if (secondCount > GlobalPerSecondLimit)
                {
                    _logger.LogDebug(
                        "Global per-second rate limit reached ({Count}/{Limit}), waiting 1 second",
                        secondCount, GlobalPerSecondLimit);
                    
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                    retryCount++;
                    continue;
                }
                
                // Check per-hour limit
                var hourKey = $"{BlizzardRateLimitKeyPrefix}:hour:{now:yyyyMMddHH}";
                var hourCount = await db.StringIncrementAsync(hourKey);
                
                if (hourCount == 1)
                {
                    // First request in this hour - set expiration
                    await db.KeyExpireAsync(hourKey, TimeSpan.FromHours(2));
                }
                
                if (hourCount > GlobalPerHourLimit)
                {
                    var waitTime = now.AddHours(1).AddMinutes(1) - now;
                    
                    _logger.LogWarning(
                        "Global per-hour rate limit reached ({Count}/{Limit}), waiting {WaitTime}",
                        hourCount, GlobalPerHourLimit, waitTime);
                    
                    await Task.Delay(waitTime, cancellationToken);
                    retryCount++;
                    continue;
                }
                
                // Both limits OK - request allowed
                return;
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "Redis error while checking Blizzard API rate limit");
                // On Redis errors, allow the request (fail open) but add a small delay
                await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
                return;
            }
        }
        
        throw new InvalidOperationException(
            "Failed to acquire Blizzard API rate limit slot after maximum retries");
    }

    /// <summary>
    /// Gets current rate limit statistics for monitoring.
    /// </summary>
    /// <param name="userId">Optional user ID to get user-specific stats.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Rate limit statistics.</returns>
    public async Task<RateLimitStats> GetStatsAsync(
        string? userId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var now = DateTimeOffset.UtcNow;
            
            // Get global Blizzard API stats
            var secondKey = $"{BlizzardRateLimitKeyPrefix}:second:{now:yyyyMMddHHmmss}";
            var hourKey = $"{BlizzardRateLimitKeyPrefix}:hour:{now:yyyyMMddHH}";
            
            var secondCountStr = await db.StringGetAsync(secondKey);
            var hourCountStr = await db.StringGetAsync(hourKey);
            
            var secondCount = secondCountStr.HasValue ? (int)secondCountStr : 0;
            var hourCount = hourCountStr.HasValue ? (int)hourCountStr : 0;
            
            // Get user-specific stats if requested
            int? userMinuteCount = null;
            int? userHourCount = null;
            
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var userMinuteKey = $"{UserRateLimitKeyPrefix}:{userId}:minute:{now:yyyyMMddHHmm}";
                var userHourKey = $"{UserRateLimitKeyPrefix}:{userId}:hour:{now:yyyyMMddHH}";
                
                var userMinuteCountStr = await db.StringGetAsync(userMinuteKey);
                var userHourCountStr = await db.StringGetAsync(userHourKey);
                
                userMinuteCount = userMinuteCountStr.HasValue ? (int)userMinuteCountStr : 0;
                userHourCount = userHourCountStr.HasValue ? (int)userHourCountStr : 0;
            }
            
            return new RateLimitStats
            {
                GlobalPerSecondCount = secondCount,
                GlobalPerSecondLimit = GlobalPerSecondLimit,
                GlobalPerSecondRemaining = Math.Max(0, GlobalPerSecondLimit - secondCount),
                GlobalPerHourCount = hourCount,
                GlobalPerHourLimit = GlobalPerHourLimit,
                GlobalPerHourRemaining = Math.Max(0, GlobalPerHourLimit - hourCount),
                GlobalHourlyUtilization = (double)hourCount / GlobalPerHourLimit * 100,
                UserPerMinuteCount = userMinuteCount,
                UserPerMinuteLimit = UserPerMinuteLimit,
                UserPerHourCount = userHourCount,
                UserPerHourLimit = UserPerHourLimit
            };
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Redis error while getting rate limit stats");
            return new RateLimitStats
            {
                GlobalPerSecondLimit = GlobalPerSecondLimit,
                GlobalPerHourLimit = GlobalPerHourLimit
            };
        }
    }

    /// <summary>
    /// Resets rate limits for a specific user. Use with caution.
    /// </summary>
    /// <param name="userId">User ID to reset.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task ResetUserLimitsAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be null or empty", nameof(userId));

        try
        {
            var db = _redis.GetDatabase();
            var now = DateTimeOffset.UtcNow;
            
            var minuteKey = $"{UserRateLimitKeyPrefix}:{userId}:minute:{now:yyyyMMddHHmm}";
            var hourKey = $"{UserRateLimitKeyPrefix}:{userId}:hour:{now:yyyyMMddHH}";
            
            await db.KeyDeleteAsync(minuteKey);
            await db.KeyDeleteAsync(hourKey);
            
            _logger.LogInformation("Reset rate limits for user: {UserId}", userId);
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Redis error while resetting user rate limits for {UserId}", userId);
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
    }
}

/// <summary>
/// Rate limit statistics record for monitoring and diagnostics.
/// </summary>
public sealed record RateLimitStats
{
    // Global Blizzard API limits (across all pods)
    public int GlobalPerSecondCount { get; init; }
    public int GlobalPerSecondLimit { get; init; }
    public int GlobalPerSecondRemaining { get; init; }
    public int GlobalPerHourCount { get; init; }
    public int GlobalPerHourLimit { get; init; }
    public int GlobalPerHourRemaining { get; init; }
    public double GlobalHourlyUtilization { get; init; }
    
    // Per-user limits (optional)
    public int? UserPerMinuteCount { get; init; }
    public int UserPerMinuteLimit { get; init; }
    public int? UserPerHourCount { get; init; }
    public int UserPerHourLimit { get; init; }
}
