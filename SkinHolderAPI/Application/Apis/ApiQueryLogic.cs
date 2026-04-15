using MapsterMapper;
using SkinHolderAPI.Application.Shared;
using SkinHolderAPI.DataService.Apis;
using SkinHolderAPI.DTOs.ApiQuery;

namespace SkinHolderAPI.Application.Apis;

public interface IApiQueryLogic
{
    Task<List<GamerPayItemDto>> GetGamerPayItemPricesAsync(List<string> itemNames);
    Task<List<CSFloatItemDto>> GetCSFloatItemPricesAsync(List<string> marketHashNames);
}

public class ApiQueryLogic(IApiQueryDataService apiQueryDataService, IMapper mapper, IConfiguration config, ILogger<ApiQueryLogic> logger) : BaseLogic(mapper, config, logger), IApiQueryLogic
{
    private readonly IApiQueryDataService _apiQueryDataService = apiQueryDataService;

    public async Task<List<GamerPayItemDto>> GetGamerPayItemPricesAsync(List<string> itemNames)
    {
        _logger.LogInformation("GetGamerPayItemPricesAsync llamado con {Count} items", itemNames.Count);

        try
        {
            var result = await _apiQueryDataService.GetGamerPayItemPricesAsync(itemNames);

            if (result.Count == 0)
                _logger.LogWarning("GetGamerPayItemPricesAsync no devolvió resultados para {Count} items solicitados", itemNames.Count);
            else
                _logger.LogInformation("GetGamerPayItemPricesAsync completado: {ResultCount}/{RequestedCount} items encontrados", result.Count, itemNames.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetGamerPayItemPricesAsync con {Count} items", itemNames.Count);
            throw;
        }
    }

    public async Task<List<CSFloatItemDto>> GetCSFloatItemPricesAsync(List<string> marketHashNames)
    {
        _logger.LogInformation("GetCSFloatItemPricesAsync llamado con {Count} items", marketHashNames.Count);

        try
        {
            var apiKeys = _config.GetSection("CSFloatKey").Get<string[]>() ?? [];

            if (apiKeys.Length == 0)
                _logger.LogWarning("No se encontraron API keys de CSFloat en la configuración");

            var result = await _apiQueryDataService.GetCSFloatItemPricesAsync(marketHashNames, apiKeys);

            if (result.Count == 0)
                _logger.LogWarning("GetCSFloatItemPricesAsync no devolvió resultados para {Count} items solicitados", marketHashNames.Count);
            else
                _logger.LogInformation("GetCSFloatItemPricesAsync completado: {ResultCount}/{RequestedCount} items encontrados", result.Count, marketHashNames.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetCSFloatItemPricesAsync con {Count} items", marketHashNames.Count);
            throw;
        }
    }
}
