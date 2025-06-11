using AutoMapper;

namespace SkinHolderAPI.Application.Shared;

public class BaseLogic(IMapper mapper, IConfiguration config)
{
    public readonly IMapper _mapper = mapper ;
    public readonly IConfiguration _config = config;
}
