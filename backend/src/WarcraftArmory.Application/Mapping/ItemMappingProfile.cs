using Mapster;
using WarcraftArmory.Application.DTOs.Responses;
using WarcraftArmory.Domain.Entities;

namespace WarcraftArmory.Application.Mapping;

/// <summary>
/// Mapster configuration for Item entity mappings.
/// </summary>
public sealed class ItemMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Item, ItemResponse>()
            .Map(dest => dest.Quality, src => src.Quality.ToString());
    }
}
