using AutoMapper;
using SkinHolderAPI.DTOs.Items;
using SkinHolderAPI.DTOs.Login;
using SkinHolderAPI.DTOs.Registros;
using SkinHolderAPI.Models;

namespace SkinHolderAPI.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Login
            CreateMap<User, LoginResultDto>();

            // Items
            CreateMap<Item, ItemDto>();

            // Registros
            CreateMap<Registro, RegistroDto>();
        }
    }
}
