using Mapster;
using SkinHolderAPI.DTOs.ItemPrecio;
using SkinHolderAPI.DTOs.Items;
using SkinHolderAPI.DTOs.Login;
using SkinHolderAPI.DTOs.Registros;
using SkinHolderAPI.DTOs.UserItemsDto;
using SkinHolderAPI.Models;

namespace SkinHolderAPI.Application;

public class MappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Login
        config.NewConfig<User, LoginResultDto>();

        // Items
        config.NewConfig<Item, ItemDto>();

        // UserItems
        config.NewConfig<Useritem, UserItemDto>()
            .Map(dest => dest.ItemName, src => src.Item.Nombre)
            .Map(dest => dest.SteamHashName, src => src.Item.Hashnamesteam)
            .Map(dest => dest.GamerPayName, src => src.Item.Gamerpaynombre)
            .Map(dest => dest.CSFloatMarketHashName, src => src.Item.Nombre);
        config.NewConfig<UserItemDto, Useritem>()
            .Map(dest => dest.Preciomediocompra, src => 0.0);

        // Registros
        config.NewConfig<Registro, RegistroDto>().TwoWays();

        // ItemPrecio
        config.NewConfig<Itemprecio, ItemPrecioDto>().TwoWays();
    }
}
