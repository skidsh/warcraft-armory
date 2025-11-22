using MediatR;
using WarcraftArmory.Application.DTOs.Responses;
using WarcraftArmory.Domain.Enums;

namespace WarcraftArmory.Application.UseCases.Characters.Queries;

/// <summary>
/// Query for getting a character by name, realm, and region.
/// </summary>
public sealed record GetCharacterQuery(string Realm, string Name, Region Region) 
    : IRequest<CharacterResponse?>;
