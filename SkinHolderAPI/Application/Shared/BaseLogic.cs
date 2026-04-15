using MapsterMapper;

namespace SkinHolderAPI.Application.Shared;

public class BaseLogic(IMapper mapper, IConfiguration config, ILogger logger)
{
    public readonly IMapper _mapper = mapper;
    public readonly IConfiguration _config = config;
    public readonly ILogger _logger = logger;
}
