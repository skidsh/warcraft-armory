using Mapster;
using WarcraftArmory.Application.DTOs.Responses;
using WarcraftArmory.Domain.Entities;

namespace WarcraftArmory.Application.Mapping;

/// <summary>
/// Mapster configuration for Guild entity mappings.
/// </summary>
public sealed class GuildMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Guild, GuildResponse>()
            .Map(dest => dest.Region, src => src.Region.ToString())
            .Map(dest => dest.Faction, src => src.Faction.ToString());
    }
}
