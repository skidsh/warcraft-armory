using Microsoft.AspNetCore.Mvc;
using WarcraftArmory.Domain.Enums;

namespace WarcraftArmory.WebApi.Controllers;

/// <summary>
/// Controller for realm-related operations.
/// </summary>
[ApiController]
[Route("api/realms")]
[Produces("application/json")]
public sealed class RealmsController : ControllerBase
{
    private readonly ILogger<RealmsController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RealmsController"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public RealmsController(ILogger<RealmsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets a realm by ID and region.
    /// </summary>
    /// <param name="region">The region (us, eu, kr, tw, cn).</param>
    /// <param name="realmId">The realm ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The realm details.</returns>
    /// <response code="200">Returns the realm details.</response>
    /// <response code="404">If the realm is not found.</response>
    /// <response code="400">If the request parameters are invalid.</response>
    /// <response code="501">Not implemented yet - planned for future release.</response>
    [HttpGet("{region}/{realmId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status501NotImplemented)]
    public async Task<IActionResult> GetRealm(
        string region,
        int realmId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Getting realm {RealmId} in region {Region}",
            realmId, region);

        // Parse region enum
        if (!Enum.TryParse<Region>(region, ignoreCase: true, out var regionEnum))
        {
            return BadRequest(new ValidationProblemDetails
            {
                Title = "Invalid region",
                Detail = $"Region '{region}' is not valid. Valid regions: us, eu, kr, tw, cn.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        // TODO: Implement realm query handler
        return StatusCode(StatusCodes.Status501NotImplemented, new ProblemDetails
        {
            Title = "Not Implemented",
            Detail = "Realm retrieval is not yet implemented. This feature is planned for a future release.",
            Status = StatusCodes.Status501NotImplemented
        });
    }

    /// <summary>
    /// Lists all realms for a region.
    /// </summary>
    /// <param name="region">The region (us, eu, kr, tw, cn).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of realms.</returns>
    /// <response code="200">Returns the list of realms.</response>
    /// <response code="400">If the request parameters are invalid.</response>
    /// <response code="501">Not implemented yet - planned for future release.</response>
    [HttpGet("{region}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status501NotImplemented)]
    public async Task<IActionResult> ListRealms(
        string region,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Listing realms in region {Region}", region);

        // Parse region enum
        if (!Enum.TryParse<Region>(region, ignoreCase: true, out var regionEnum))
        {
            return BadRequest(new ValidationProblemDetails
            {
                Title = "Invalid region",
                Detail = $"Region '{region}' is not valid. Valid regions: us, eu, kr, tw, cn.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        // TODO: Implement list realms query handler
        return StatusCode(StatusCodes.Status501NotImplemented, new ProblemDetails
        {
            Title = "Not Implemented",
            Detail = "Realm listing is not yet implemented. This feature is planned for a future release.",
            Status = StatusCodes.Status501NotImplemented
        });
    }
}
