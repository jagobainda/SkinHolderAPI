using AutoMapper;
using SkinHolderAPI.DTOs.Items;
using SkinHolderAPI.DTOs.Login;
using SkinHolderAPI.Models;

namespace SkinHolderAPI.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Login
            CreateMap<User, LoginResultDto>();

            //Items
            CreateMap<Item, ItemDto>();
        }
    }
}
