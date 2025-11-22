using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WarcraftArmory.Infrastructure.ExternalServices.BlizzardApi;

namespace WarcraftArmory.WebApi.Middleware;

/// <summary>
/// Per-user rate limiting middleware to prevent API abuse.
/// Uses distributed rate limiter (Redis) for coordination across multiple pods.
/// </summary>
public sealed class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly DistributedRateLimiter _rateLimiter;
    private readonly ILogger<RateLimitingMiddleware> _logger;

    public RateLimitingMiddleware(
        RequestDelegate next,
        DistributedRateLimiter rateLimiter,
        ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _rateLimiter = rateLimiter;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip rate limiting for health check endpoints
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        // Get user identifier (IP address or authenticated user ID)
        var userIdentifier = GetUserIdentifier(context);

        // Check if user is allowed to make a request
        var isAllowed = await _rateLimiter.IsUserRequestAllowedAsync(userIdentifier);

        if (!isAllowed)
        {
            _logger.LogWarning(
                "Rate limit exceeded for user {UserIdentifier} on path {Path}",
                userIdentifier,
                context.Request.Path);

            await HandleRateLimitExceeded(context, userIdentifier);
            return;
        }

        // Get rate limit stats for response headers
        var stats = await _rateLimiter.GetStatsAsync(userIdentifier);

        // Add rate limit headers to response
        context.Response.OnStarting(() =>
        {
            context.Response.Headers["X-RateLimit-Limit-Minute"] = stats.UserPerMinuteLimit.ToString();
            context.Response.Headers["X-RateLimit-Limit-Hour"] = stats.UserPerHourLimit.ToString();
            context.Response.Headers["X-RateLimit-Used-Minute"] = (stats.UserPerMinuteCount ?? 0).ToString();
            context.Response.Headers["X-RateLimit-Used-Hour"] = (stats.UserPerHourCount ?? 0).ToString();
            return Task.CompletedTask;
        });

        await _next(context);
    }

    private static string GetUserIdentifier(HttpContext context)
    {
        // Try to get authenticated user ID first
        var userId = context.User?.Identity?.Name;
        if (!string.IsNullOrWhiteSpace(userId))
        {
            return $"user:{userId}";
        }

        // Fall back to IP address
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        
        // Check for X-Forwarded-For header (when behind proxy/load balancer)
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            var ips = forwardedFor.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (ips.Length > 0)
            {
                ipAddress = ips[0].Trim();
            }
        }

        return $"ip:{ipAddress}";
    }

    private static async Task HandleRateLimitExceeded(HttpContext context, string userIdentifier)
    {
        context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
        context.Response.ContentType = "application/problem+json";

        var stats = await context.RequestServices
            .GetRequiredService<DistributedRateLimiter>()
            .GetStatsAsync(userIdentifier);

        var problemDetails = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc6585#section-4",
            Title = "Too many requests",
            Status = (int)HttpStatusCode.TooManyRequests,
            Detail = "Rate limit exceeded. Please try again later.",
            Instance = context.Request.Path
        };

        problemDetails.Extensions["rateLimitType"] = "per-user";
        problemDetails.Extensions["limitPerMinute"] = stats.UserPerMinuteLimit;
        problemDetails.Extensions["limitPerHour"] = stats.UserPerHourLimit;
        problemDetails.Extensions["usedPerMinute"] = stats.UserPerMinuteCount ?? 0;
        problemDetails.Extensions["usedPerHour"] = stats.UserPerHourCount ?? 0;
        problemDetails.Extensions["retryAfter"] = CalculateRetryAfter();

        // Add Retry-After header (RFC 6585) - suggest 60 seconds
        context.Response.Headers["Retry-After"] = "60";

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
    }

    private static int CalculateRetryAfter()
    {
        // Suggest waiting 60 seconds before retrying
        return 60;
    }
}

/// <summary>
/// Extension methods for registering the rate limiting middleware.
/// </summary>
public static class RateLimitingMiddlewareExtensions
{
    /// <summary>
    /// Adds the rate limiting middleware to the application pipeline.
    /// This should be registered after exception handling but before authorization.
    /// </summary>
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RateLimitingMiddleware>();
    }
}
