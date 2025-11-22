using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WarcraftArmory.Application.DTOs.Requests;
using WarcraftArmory.Application.DTOs.Responses;
using WarcraftArmory.Application.UseCases.Guilds.Queries;
using WarcraftArmory.Domain.Enums;

namespace WarcraftArmory.WebApi.Controllers;

/// <summary>
/// Controller for guild-related operations.
/// </summary>
[ApiController]
[Route("api/guilds")]
[Produces("application/json")]
public sealed class GuildsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<GuildsController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GuildsController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator.</param>
    /// <param name="mapper">The mapper.</param>
    /// <param name="logger">The logger.</param>
    public GuildsController(
        IMediator mediator,
        IMapper mapper,
        ILogger<GuildsController> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Gets a guild by realm, name, and region.
    /// </summary>
    /// <param name="region">The region (us, eu, kr, tw, cn).</param>
    /// <param name="realm">The realm slug.</param>
    /// <param name="name">The guild name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The guild details.</returns>
    /// <response code="200">Returns the guild details.</response>
    /// <response code="404">If the guild is not found.</response>
    /// <response code="400">If the request parameters are invalid.</response>
    [HttpGet("{region}/{realm}/{name}")]
    [ProducesResponseType(typeof(GuildResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GuildResponse>> GetGuild(
        string region,
        string realm,
        string name,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Getting guild {Name} from realm {Realm} in region {Region}",
            name, realm, region);

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

        var request = new GetGuildRequest
        {
            Realm = realm,
            Name = name,
            Region = regionEnum
        };

        var query = _mapper.Map<GetGuildQuery>(request);
        var response = await _mediator.Send(query, cancellationToken);

        if (response == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Guild not found",
                Detail = $"Guild '{name}' was not found on realm '{realm}' in region '{region}'.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(response);
    }
}
