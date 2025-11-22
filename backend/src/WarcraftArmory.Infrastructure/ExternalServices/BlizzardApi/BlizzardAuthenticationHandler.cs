using System.Net.Http.Headers;

namespace WarcraftArmory.Infrastructure.ExternalServices.BlizzardApi;

/// <summary>
/// HTTP message handler that adds OAuth Bearer token authentication to Blizzard API requests.
/// Automatically retrieves and attaches access tokens from BlizzardAuthService.
/// </summary>
public sealed class BlizzardAuthenticationHandler : DelegatingHandler
{
    private readonly BlizzardAuthService _authService;

    public BlizzardAuthenticationHandler(BlizzardAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Get access token from auth service
        var accessToken = await _authService.GetAccessTokenAsync(cancellationToken);

        // Add Bearer token to Authorization header
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // Continue with the request
        return await base.SendAsync(request, cancellationToken);
    }
}
