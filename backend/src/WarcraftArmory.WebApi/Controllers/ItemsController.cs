using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WarcraftArmory.Application.DTOs.Requests;
using WarcraftArmory.Application.DTOs.Responses;
using WarcraftArmory.Application.UseCases.Items.Queries;
using WarcraftArmory.Domain.Enums;

namespace WarcraftArmory.WebApi.Controllers;

/// <summary>
/// Controller for item-related operations.
/// </summary>
[ApiController]
[Route("api/items")]
[Produces("application/json")]
public sealed class ItemsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<ItemsController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemsController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator.</param>
    /// <param name="mapper">The mapper.</param>
    /// <param name="logger">The logger.</param>
    public ItemsController(
        IMediator mediator,
        IMapper mapper,
        ILogger<ItemsController> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Gets an item by ID and region.
    /// </summary>
    /// <param name="region">The region (us, eu, kr, tw, cn).</param>
    /// <param name="itemId">The item ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The item details.</returns>
    /// <response code="200">Returns the item details.</response>
    /// <response code="404">If the item is not found.</response>
    /// <response code="400">If the request parameters are invalid.</response>
    [HttpGet("{region}/{itemId:int}")]
    [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ItemResponse>> GetItem(
        string region,
        int itemId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Getting item {ItemId} in region {Region}",
            itemId, region);

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

        var request = new GetItemRequest
        {
            ItemId = itemId,
            Region = regionEnum
        };

        var query = _mapper.Map<GetItemQuery>(request);
        var response = await _mediator.Send(query, cancellationToken);

        if (response == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Item not found",
                Detail = $"Item with ID '{itemId}' was not found in region '{region}'.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(response);
    }
}
