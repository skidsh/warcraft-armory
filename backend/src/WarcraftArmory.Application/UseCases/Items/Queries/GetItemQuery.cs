using MediatR;
using WarcraftArmory.Application.DTOs.Responses;
using WarcraftArmory.Domain.Enums;

namespace WarcraftArmory.Application.UseCases.Items.Queries;

/// <summary>
/// Query for getting an item by ID and region.
/// </summary>
public sealed record GetItemQuery(int ItemId, Region Region) 
    : IRequest<ItemResponse?>;
