using Mapster;
using WarcraftArmory.Application.DTOs.Responses;
using WarcraftArmory.Domain.Entities;

namespace WarcraftArmory.Application.Mapping;

/// <summary>
/// Mapster configuration for Character entity mappings.
/// </summary>
public sealed class CharacterMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Character, CharacterResponse>()
            .Map(dest => dest.Region, src => src.Region.ToString())
            .Map(dest => dest.Class, src => src.Class.ToString())
            .Map(dest => dest.Race, src => src.Race.ToString())
            .Map(dest => dest.Faction, src => src.Faction.ToString())
            .Map(dest => dest.Gender, src => src.Gender.ToString());
    }
}
