namespace WarcraftArmory.Infrastructure.ExternalServices.Configuration;

/// <summary>
/// Configuration settings for the Blizzard Battle.net API.
/// These settings should be loaded from appsettings.json or User Secrets.
/// </summary>
public sealed record BlizzardApiSettings
{
    /// <summary>
    /// OAuth 2.0 Client ID from Battle.net Developer Portal.
    /// </summary>
    public required string ClientId { get; init; }

    /// <summary>
    /// OAuth 2.0 Client Secret from Battle.net Developer Portal.
    /// IMPORTANT: Never commit this value to source control.
    /// Use User Secrets (local) or AWS Secrets Manager (production).
    /// </summary>
    public required string ClientSecret { get; init; }

    /// <summary>
    /// Default region for API requests (us, eu, kr, tw, cn).
    /// </summary>
    public string DefaultRegion { get; init; } = "us";

    /// <summary>
    /// Base URL for the OAuth token endpoint.
    /// Default: https://oauth.battle.net
    /// </summary>
    public string OAuthBaseUrl { get; init; } = "https://oauth.battle.net";

    /// <summary>
    /// Base URL template for Game Data API requests.
    /// Use {region} placeholder for dynamic region injection.
    /// Default: https://{region}.api.blizzard.com
    /// </summary>
    public string ApiBaseUrl { get; init; } = "https://{region}.api.blizzard.com";

    /// <summary>
    /// Rate limit: Maximum requests per second.
    /// Blizzard API limit: 100 requests/second.
    /// </summary>
    public int MaxRequestsPerSecond { get; init; } = 100;

    /// <summary>
    /// Rate limit: Maximum requests per hour.
    /// Blizzard API limit: 36,000 requests/hour.
    /// </summary>
    public int MaxRequestsPerHour { get; init; } = 36000;

    /// <summary>
    /// HTTP request timeout in seconds.
    /// </summary>
    public int RequestTimeoutSeconds { get; init; } = 30;

    /// <summary>
    /// Number of retry attempts for failed requests.
    /// </summary>
    public int RetryAttempts { get; init; } = 3;

    /// <summary>
    /// Exponential backoff base delay in milliseconds.
    /// </summary>
    public int RetryDelayMilliseconds { get; init; } = 1000;

    /// <summary>
    /// Circuit breaker: Number of consecutive failures before opening the circuit.
    /// </summary>
    public int CircuitBreakerFailureThreshold { get; init; } = 5;

    /// <summary>
    /// Circuit breaker: Duration in seconds to keep the circuit open.
    /// </summary>
    public int CircuitBreakerDurationSeconds { get; init; } = 30;

    /// <summary>
    /// Gets the API base URL for a specific region.
    /// </summary>
    /// <param name="region">Region code (us, eu, kr, tw, cn).</param>
    /// <returns>Formatted API base URL.</returns>
    public string GetApiBaseUrl(string region)
    {
        return ApiBaseUrl.Replace("{region}", region.ToLowerInvariant());
    }

    /// <summary>
    /// Validates that required settings are configured.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ClientId))
        {
            throw new InvalidOperationException("BlizzardApi:ClientId is not configured.");
        }

        if (string.IsNullOrWhiteSpace(ClientSecret))
        {
            throw new InvalidOperationException("BlizzardApi:ClientSecret is not configured.");
        }

        if (string.IsNullOrWhiteSpace(DefaultRegion))
        {
            throw new InvalidOperationException("BlizzardApi:DefaultRegion is not configured.");
        }

        if (MaxRequestsPerSecond <= 0)
        {
            throw new InvalidOperationException("BlizzardApi:MaxRequestsPerSecond must be greater than 0.");
        }

        if (MaxRequestsPerHour <= 0)
        {
            throw new InvalidOperationException("BlizzardApi:MaxRequestsPerHour must be greater than 0.");
        }
    }
}
