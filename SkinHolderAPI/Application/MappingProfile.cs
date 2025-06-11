using AutoMapper;
using SkinHolderAPI.DTOs.Login;
using SkinHolderAPI.Models;

namespace SkinHolderAPI.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, LoginResultDto>();
        }
    }
}
