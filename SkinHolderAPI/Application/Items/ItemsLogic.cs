using MapsterMapper;
using SkinHolderAPI.Application.Shared;
using SkinHolderAPI.DataService.Items;
using SkinHolderAPI.DTOs.Items;

namespace SkinHolderAPI.Application.Items;

public interface IItemsLogic
{
    Task<List<ItemDto>> GetItemsAsync();
}

public class ItemsLogic(IItemsDataService itemsDataService, IMapper mapper, IConfiguration config, ILogger<ItemsLogic> logger) : BaseLogic(mapper, config, logger), IItemsLogic
{
    private readonly IItemsDataService _itemsDataService = itemsDataService;

    public async Task<List<ItemDto>> GetItemsAsync()
    {
        _logger.LogInformation("GetItemsAsync llamado");

        try
        {
            var items = await _itemsDataService.GetItemsAsync();

            if (items == null || items.Count == 0)
            {
                _logger.LogWarning("GetItemsAsync no devolvió items");
                return [];
            }

            var result = items.Select(i => _mapper.Map<ItemDto>(i)).OrderBy(i => i.Nombre).ToList();

            _logger.LogInformation("GetItemsAsync completado: {Count} items devueltos", result.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetItemsAsync");
            throw;
        }
    }
}
