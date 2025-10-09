using AutoMapper;
using SkinHolderAPI.DTOs.ItemPrecio;
using SkinHolderAPI.DTOs.Items;
using SkinHolderAPI.DTOs.Login;
using SkinHolderAPI.DTOs.Registros;
using SkinHolderAPI.DTOs.UserItemsDto;
using SkinHolderAPI.Models;

namespace SkinHolderAPI.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Login
        CreateMap<User, LoginResultDto>();

        // Items
        CreateMap<Item, ItemDto>();

        // UserItems
        CreateMap<Useritem, UserItemDto>()
            .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item.Nombre))
            .ForMember(dest => dest.SteamHashName, opt => opt.MapFrom(src => src.Item.Hashnamesteam))
            .ForMember(dest => dest.GamerPayName, opt => opt.MapFrom(src => src.Item.Gamerpaynombre))
            .ForMember(dest => dest.CSFloatMarketHashName, opt => opt.MapFrom(src => src.Item.Nombre));
        CreateMap<UserItemDto, Useritem>().ForMember(dest => dest.Preciomediocompra, opt => opt.MapFrom(src => 0.0));

        // Registros
        CreateMap<Registro, RegistroDto>().ReverseMap();

        // ItemPrecio
        CreateMap<Itemprecio, ItemPrecioDto>().ReverseMap();
    }
}
