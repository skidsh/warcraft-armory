using System.Text.Json.Serialization;

namespace WarcraftArmory.Infrastructure.ExternalServices.Configuration;

/// <summary>
/// Represents the OAuth 2.0 token response from Blizzard's token endpoint.
/// </summary>
public sealed record OAuthTokenResponse
{
    /// <summary>
    /// The access token for authenticating API requests.
    /// </summary>
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    /// <summary>
    /// The token type, typically "Bearer".
    /// </summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = "Bearer";

    /// <summary>
    /// Token expiration time in seconds from issuance.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }

    /// <summary>
    /// The scopes granted for this token (optional).
    /// </summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; init; }

    /// <summary>
    /// Timestamp when the token was issued (for internal tracking).
    /// Not part of the API response - set by the client.
    /// </summary>
    [JsonIgnore]
    public DateTime IssuedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Calculated expiration timestamp.
    /// </summary>
    [JsonIgnore]
    public DateTime ExpiresAt => IssuedAt.AddSeconds(ExpiresIn);

    /// <summary>
    /// Checks if the token is expired or about to expire.
    /// </summary>
    /// <param name="bufferSeconds">Buffer time in seconds before actual expiration. Default: 60 seconds.</param>
    /// <returns>True if the token is expired or about to expire, otherwise false.</returns>
    public bool IsExpired(int bufferSeconds = 60)
    {
        return DateTime.UtcNow >= ExpiresAt.AddSeconds(-bufferSeconds);
    }

    /// <summary>
    /// Gets the remaining time until expiration.
    /// </summary>
    /// <returns>TimeSpan representing remaining time, or TimeSpan.Zero if expired.</returns>
    public TimeSpan GetRemainingTime()
    {
        var remaining = ExpiresAt - DateTime.UtcNow;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }

    /// <summary>
    /// Creates a formatted Authorization header value.
    /// </summary>
    /// <returns>Authorization header value (e.g., "Bearer abc123...").</returns>
    public string ToAuthorizationHeaderValue()
    {
        return $"{TokenType} {AccessToken}";
    }
}
