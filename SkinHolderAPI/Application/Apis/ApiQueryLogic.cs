using AutoMapper;
using SkinHolderAPI.Application.Shared;
using SkinHolderAPI.DataService.Apis;
using SkinHolderAPI.DTOs.ApiQuery;

namespace SkinHolderAPI.Application.Apis;

public interface IApiQueryLogic
{
    Task<List<GamerPayItemDto>> GetGamerPayItemPricesAsync(List<string> itemNames);
    Task<List<CSFloatItemDto>> GetCSFloatItemPricesAsync(List<string> marketHashNames);
}

public class ApiQueryLogic(IApiQueryDataService apiQueryDataService, IMapper mapper, IConfiguration config) : BaseLogic(mapper, config), IApiQueryLogic
{
    private readonly IApiQueryDataService _apiQueryDataService = apiQueryDataService;

    public async Task<List<GamerPayItemDto>> GetGamerPayItemPricesAsync(List<string> itemNames)
    {
        return await _apiQueryDataService.GetGamerPayItemPricesAsync(itemNames);
    }

    public async Task<List<CSFloatItemDto>> GetCSFloatItemPricesAsync(List<string> marketHashNames)
    {
        var apiKeys = _config.GetSection("CSFloatKey").Get<string[]>() ?? [];

        return await _apiQueryDataService.GetCSFloatItemPricesAsync(marketHashNames, apiKeys);
    }
}
