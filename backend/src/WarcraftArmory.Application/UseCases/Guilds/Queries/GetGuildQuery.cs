using MediatR;
using WarcraftArmory.Application.DTOs.Responses;
using WarcraftArmory.Domain.Enums;

namespace WarcraftArmory.Application.UseCases.Guilds.Queries;

/// <summary>
/// Query for getting a guild by name, realm, and region.
/// </summary>
public sealed record GetGuildQuery(string Realm, string Name, Region Region) 
    : IRequest<GuildResponse?>;
