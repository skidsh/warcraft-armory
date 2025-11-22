using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WarcraftArmory.Application.DTOs.Requests;
using WarcraftArmory.Application.DTOs.Responses;
using WarcraftArmory.Application.UseCases.Characters.Queries;
using WarcraftArmory.Domain.Enums;

namespace WarcraftArmory.WebApi.Controllers;

/// <summary>
/// Controller for character-related operations.
/// </summary>
[ApiController]
[Route("api/characters")]
[Produces("application/json")]
public sealed class CharactersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<CharactersController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CharactersController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator.</param>
    /// <param name="mapper">The mapper.</param>
    /// <param name="logger">The logger.</param>
    public CharactersController(
        IMediator mediator,
        IMapper mapper,
        ILogger<CharactersController> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Gets a character by realm, name, and region.
    /// </summary>
    /// <param name="region">The region (us, eu, kr, tw, cn).</param>
    /// <param name="realm">The realm slug.</param>
    /// <param name="name">The character name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The character details.</returns>
    /// <response code="200">Returns the character details.</response>
    /// <response code="404">If the character is not found.</response>
    /// <response code="400">If the request parameters are invalid.</response>
    [HttpGet("{region}/{realm}/{name}")]
    [ProducesResponseType(typeof(CharacterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CharacterResponse>> GetCharacter(
        string region,
        string realm,
        string name,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Getting character {Name} from realm {Realm} in region {Region}",
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

        var request = new GetCharacterRequest
        {
            Realm = realm,
            Name = name,
            Region = regionEnum
        };

        var query = _mapper.Map<GetCharacterQuery>(request);
        var response = await _mediator.Send(query, cancellationToken);

        if (response == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Character not found",
                Detail = $"Character '{name}' was not found on realm '{realm}' in region '{region}'.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(response);
    }
}
