using Mapster;
using WarcraftArmory.Application.DTOs.Requests;
using WarcraftArmory.Application.UseCases.Characters.Queries;
using WarcraftArmory.Application.UseCases.Guilds.Queries;
using WarcraftArmory.Application.UseCases.Items.Queries;

namespace WarcraftArmory.Application.Mapping;

/// <summary>
/// Mapster configuration for mapping Request DTOs to Query objects.
/// </summary>
public sealed class RequestToQueryMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // GetCharacterRequest -> GetCharacterQuery
        config.NewConfig<GetCharacterRequest, GetCharacterQuery>()
            .Map(dest => dest.Realm, src => src.Realm)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Region, src => src.Region);

        // GetItemRequest -> GetItemQuery
        config.NewConfig<GetItemRequest, GetItemQuery>()
            .Map(dest => dest.ItemId, src => src.ItemId)
            .Map(dest => dest.Region, src => src.Region);

        // GetGuildRequest -> GetGuildQuery
        config.NewConfig<GetGuildRequest, GetGuildQuery>()
            .Map(dest => dest.Realm, src => src.Realm)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Region, src => src.Region);
    }
}
