# Rate Limiting & Caching Architecture for Distributed Applications

## Overview

This document explains the design decisions for rate limiting and caching in a multi-pod/container deployment.

---

## 1. Rate Limiting Strategy

### Problem with Original Design
The original `RateLimiter` used in-memory data structures (`ConcurrentQueue`, `SemaphoreSlim`) which are **per-pod only**. In a distributed deployment:
- Each pod tracks limits independently
- 3 pods × 100 req/sec = **300 req/sec total** (exceeds Blizzard's 100 req/sec limit)
- **Risk of hitting Blizzard's rate limits and getting blocked**

### Solution: `DistributedRateLimiter`

Uses **Redis** for distributed rate limiting across all pods.

#### Two-Level Rate Limiting:

1. **Global Blizzard API Rate Limiting** (All Pods Combined)
   - **80 requests/second** (conservative, 80% of Blizzard's 100 limit)
   - **28,800 requests/hour** (80% of Blizzard's 36,000 limit)
   - Ensures all pods combined stay within safe limits
   - Redis keys: `ratelimit:blizzard:second:yyyyMMddHHmmss` and `ratelimit:blizzard:hour:yyyyMMddHH`

2. **Per-User Rate Limiting** (Prevents Abuse)
   - **60 requests/minute per user** (1 req/sec per user)
   - **1,000 requests/hour per user** (~16 req/min per user)
   - Protects against individual users overwhelming the API
   - User identifier: IP address, user ID, or API key
   - Redis keys: `ratelimit:user:{userId}:minute:yyyyMMddHHmm` and `ratelimit:user:{userId}:hour:yyyyMMddHH`

#### Usage Pattern:

```csharp
// In API Controllers - Check user rate limit first
public async Task<IActionResult> GetCharacter(...)
{
    var userId = GetUserIdentifier(); // IP or user ID
    
    if (!await _rateLimiter.IsUserRequestAllowedAsync(userId))
    {
        return StatusCode(429, "Rate limit exceeded. Please try again later.");
    }
    
    // Process request...
}

// In BlizzardApiService - Check global limit before calling Blizzard
public async Task<Character> GetCharacterAsync(...)
{
    // Wait for global rate limit slot
    await _rateLimiter.WaitForBlizzardApiSlotAsync(cancellationToken);
    
    // Call Blizzard API
    var result = await _apiClient.GetCharacterAsync(...);
    
    return result;
}
```

#### Key Features:
- ✅ **Distributed** - Works across multiple pods using Redis
- ✅ **Conservative** - Targets 80% of Blizzard's limits for safety margin
- ✅ **Per-User Protection** - Prevents individual users from abuse
- ✅ **Fail-Open** - On Redis errors, allows requests (availability over strict limiting)
- ✅ **Sliding Window** - Uses time-based keys for accurate rate tracking
- ✅ **Auto-Expiration** - Redis keys expire automatically, no cleanup needed

---

## 2. Caching Strategy

### Two-Tier Caching Architecture

The application uses **both** Memory Cache and Redis Cache intentionally:

#### Tier 1: Memory Cache (`MemoryCacheService`)
- **Scope:** Per-pod (isolated to each container instance)
- **Purpose:** Ultra-fast access for hot data
- **Use Cases:**
  - OAuth access tokens (each pod authenticates independently)
  - Frequently accessed game data (reduce Redis roundtrips)
  - Short TTL (1-5 minutes)
  
#### Tier 2: Redis Cache (`RedisCacheService`)
- **Scope:** Shared across all pods (distributed)
- **Purpose:** Persistent, shared cache
- **Use Cases:**
  - Game data from Blizzard API (characters, items, guilds)
  - Longer TTL (30 minutes to 7 days depending on data type)
  - Shared results across all pods

### Cache Flow Diagram

```
User Request
    ↓
[API Controller] → Check user rate limit
    ↓
[BlizzardApiService]
    ↓
Check Memory Cache (Pod-local)
    ↓ (miss)
Check Redis Cache (Distributed)
    ↓ (miss)
Wait for Global Rate Limit Slot
    ↓
Call Blizzard API
    ↓
Store in Redis Cache
    ↓
Store in Memory Cache
    ↓
Return to User
```

### Why This Design is Correct

**Memory Cache per-pod is intentional and beneficial:**

1. **Reduces Network Calls**
   - Hot data (e.g., most popular character) hits memory first
   - No Redis network roundtrip (saves 1-5ms per request)
   - In 3-pod deployment: Each pod caches independently, but reduces Redis load by 3x

2. **OAuth Token Caching**
   - Each pod authenticates with Blizzard independently
   - Tokens cached per-pod (no sharing needed)
   - If one pod restarts, others keep their tokens

3. **Natural Cache Warming**
   - Each pod builds its own hot data cache based on its traffic
   - Popular items naturally get cached in all pods
   - Less popular items may only be in Redis

4. **Fault Tolerance**
   - If Redis fails temporarily, pods can still serve from memory cache
   - Gradual degradation instead of complete failure

### Cache TTL Strategy

| Data Type | Memory Cache TTL | Redis Cache TTL | Rationale |
|-----------|------------------|-----------------|-----------|
| OAuth Tokens | Until expiry (~20 min) | N/A | Per-pod authentication |
| Static Data (items, spells) | 5 minutes | 7 days | Rarely changes |
| Dynamic Data (auction, token) | N/A | 5-15 minutes | Frequently changes |
| Profile Data (characters, guilds) | 2 minutes | 30-60 minutes | Semi-static |
| Search Results | N/A | 10-30 minutes | Moderate freshness |

### Configuration Example

```csharp
// In Startup/Program.cs

// Tier 1: Memory Cache (per-pod)
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<MemoryCacheService>();

// Tier 2: Redis Cache (distributed)
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(
        builder.Configuration.GetConnectionString("Redis"));
    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddSingleton<RedisCacheService>();

// Rate Limiter (distributed)
builder.Services.AddSingleton<DistributedRateLimiter>();
```

---

## 3. Monitoring & Observability

### Key Metrics to Track

1. **Rate Limit Metrics**
   - Global Blizzard API utilization (should stay < 80%)
   - Per-user rate limit violations
   - Rate limit wait times
   
2. **Cache Metrics**
   - Memory cache hit rate per pod
   - Redis cache hit rate
   - Cache misses requiring Blizzard API calls
   - Average response times (memory vs Redis vs API)

3. **Blizzard API Metrics**
   - Total API calls per hour (should be < 28,800)
   - API call latency
   - API error rates

### Sample Monitoring Query

```csharp
// Get rate limit stats
var stats = await _rateLimiter.GetStatsAsync(userId: "user123");

_logger.LogInformation(
    "Rate Limit Stats - Global: {GlobalHourly}/{GlobalLimit} ({GlobalUtil:F2}%), User: {UserHourly}/{UserLimit}",
    stats.GlobalPerHourCount,
    stats.GlobalPerHourLimit,
    stats.GlobalHourlyUtilization,
    stats.UserPerHourCount ?? 0,
    stats.UserPerHourLimit);
```

---

## 4. Scaling Considerations

### Horizontal Scaling (Multiple Pods)

✅ **Rate Limiter**: Works correctly
- Redis-based distributed tracking
- All pods share the same limits

✅ **Redis Cache**: Works correctly
- Shared across all pods
- Reduces total Blizzard API calls

✅ **Memory Cache**: Works correctly
- Per-pod caching is intentional
- Each pod builds its own hot cache
- Benefits compound with more pods

### What Happens When Scaling Up?

**Scenario: Scale from 3 to 6 pods**

**Rate Limiting:**
- ✅ Global limit stays at 28,800 req/hour (distributed across 6 pods)
- ✅ User limits stay at 1,000 req/hour per user (enforced globally)

**Caching:**
- ✅ Redis cache is shared by all 6 pods
- ✅ Each new pod starts with empty memory cache
- ✅ Memory cache warms up naturally based on traffic
- ✅ Total cache hit rate improves (more memory caches, same Redis cache)

**Blizzard API Calls:**
- ✅ Total API calls stay within limits (distributed rate limiter ensures this)
- ✅ Cache hit rate may improve (more pods = more distributed memory caches for hot data)

---

## 5. Production Recommendations

1. **Set Conservative Limits**
   - Current: 80% of Blizzard's limits
   - Adjust based on monitoring

2. **Use User Identification**
   - IP address for anonymous users
   - User ID for authenticated users
   - API key for programmatic access

3. **Monitor Redis Health**
   - Set up Redis alerts for connection failures
   - Configure Redis persistence (AOF or RDB)
   - Use Redis Cluster for high availability

4. **Cache Warming**
   - Implement background job to pre-cache popular items
   - Warm cache after pod restarts

5. **Rate Limit Responses**
   - Return HTTP 429 with `Retry-After` header
   - Include rate limit info in response headers:
     - `X-RateLimit-Limit: 60`
     - `X-RateLimit-Remaining: 45`
     - `X-RateLimit-Reset: 1700000000`

---

## Summary

**Rate Limiting:**
- ✅ Use `DistributedRateLimiter` (Redis-based)
- ✅ Global Blizzard API limits (across all pods)
- ✅ Per-user limits (prevent abuse)
- ✅ Conservative targets (80% of Blizzard's limits)

**Caching:**
- ✅ Two-tier strategy is **correct and intentional**
- ✅ Memory Cache per-pod (fast, hot data)
- ✅ Redis Cache distributed (shared, persistent)
- ✅ Both work together for optimal performance

This architecture ensures your application scales horizontally while staying within Blizzard's rate limits and providing optimal performance.
