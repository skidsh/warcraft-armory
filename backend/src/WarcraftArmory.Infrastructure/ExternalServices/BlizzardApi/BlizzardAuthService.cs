using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WarcraftArmory.Infrastructure.ExternalServices.Configuration;

namespace WarcraftArmory.Infrastructure.ExternalServices.BlizzardApi;

/// <summary>
/// Service for managing OAuth 2.0 authentication with the Blizzard Battle.net API.
/// Implements client credentials flow with token caching and automatic refresh.
/// </summary>
public sealed class BlizzardAuthService
{
    private const string TokenCacheKey = "BlizzardOAuthToken";
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<BlizzardAuthService> _logger;
    private readonly BlizzardApiSettings _settings;
    private readonly SemaphoreSlim _tokenRefreshLock = new(1, 1);

    public BlizzardAuthService(
        HttpClient httpClient,
        IMemoryCache memoryCache,
        ILogger<BlizzardAuthService> logger,
        IOptions<BlizzardApiSettings> settings)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));

        // Configure HttpClient base address
        _httpClient.BaseAddress = new Uri(_settings.OAuthBaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_settings.RequestTimeoutSeconds);
    }

    /// <summary>
    /// Gets a valid access token, retrieving from cache or requesting a new one if needed.
    /// Thread-safe implementation with semaphore locking.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Valid access token string.</returns>
    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        // Try to get cached token first
        if (_memoryCache.TryGetValue<OAuthTokenResponse>(TokenCacheKey, out var cachedToken))
        {
            if (cachedToken != null && !cachedToken.IsExpired())
            {
                _logger.LogDebug("Using cached OAuth token. Expires in: {RemainingTime}", 
                    cachedToken.GetRemainingTime());
                return cachedToken.AccessToken;
            }

            _logger.LogDebug("Cached OAuth token is expired or about to expire");
        }

        // Token not cached or expired - acquire new token with locking
        await _tokenRefreshLock.WaitAsync(cancellationToken);
        try
        {
            // Double-check after acquiring lock (another thread might have refreshed)
            if (_memoryCache.TryGetValue<OAuthTokenResponse>(TokenCacheKey, out cachedToken))
            {
                if (cachedToken != null && !cachedToken.IsExpired())
                {
                    _logger.LogDebug("Token was refreshed by another thread");
                    return cachedToken.AccessToken;
                }
            }

            // Request new token
            var token = await RequestNewTokenAsync(cancellationToken);
            
            // Cache the token with expiration
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(token.GetRemainingTime());

            _memoryCache.Set(TokenCacheKey, token, cacheEntryOptions);

            _logger.LogInformation("Successfully obtained new OAuth token. Expires at: {ExpiresAt} UTC", 
                token.ExpiresAt);

            return token.AccessToken;
        }
        finally
        {
            _tokenRefreshLock.Release();
        }
    }

    /// <summary>
    /// Requests a new OAuth token from the Blizzard token endpoint.
    /// Uses client credentials flow (grant_type=client_credentials).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>OAuth token response.</returns>
    private async Task<OAuthTokenResponse> RequestNewTokenAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Requesting new OAuth token from Blizzard API");

        try
        {
            // Prepare request with Basic authentication
            var request = new HttpRequestMessage(HttpMethod.Post, "/token")
            {
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                })
            };

            // Add Basic Authentication header (Base64 encoded ClientId:ClientSecret)
            var credentials = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            // Send request
            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError(
                    "Failed to obtain OAuth token. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode,
                    errorContent);
                throw new HttpRequestException(
                    $"Failed to obtain OAuth token. Status: {response.StatusCode}");
            }

            // Deserialize response
            var tokenJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var token = JsonSerializer.Deserialize<OAuthTokenResponse>(tokenJson, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (token == null || string.IsNullOrWhiteSpace(token.AccessToken))
            {
                _logger.LogError("Received invalid token response from Blizzard API");
                throw new InvalidOperationException("Received invalid token response");
            }

            return token;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while requesting OAuth token");
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize OAuth token response");
            throw new InvalidOperationException("Failed to parse OAuth token response", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while requesting OAuth token");
            throw;
        }
    }

    /// <summary>
    /// Forces a token refresh by clearing the cache and requesting a new token.
    /// Useful for testing or recovering from authentication errors.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>New access token.</returns>
    public async Task<string> RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Forcing OAuth token refresh");
        _memoryCache.Remove(TokenCacheKey);
        return await GetAccessTokenAsync(cancellationToken);
    }

    /// <summary>
    /// Clears the cached token. Next call to GetAccessTokenAsync will request a new token.
    /// </summary>
    public void ClearCachedToken()
    {
        _logger.LogDebug("Clearing cached OAuth token");
        _memoryCache.Remove(TokenCacheKey);
    }
}
