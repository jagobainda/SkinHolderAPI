using AutoMapper;
using SkinHolderAPI.Application.Shared;
using SkinHolderAPI.DataService.Items;
using SkinHolderAPI.DTOs.Items;

namespace SkinHolderAPI.Application.Items;

public interface IItemsLogic
{
    Task<List<ItemDto>> GetItemsAsync();
}

public class ItemsLogic(IItemsDataService itemsDataService, IMapper mapper, IConfiguration config) : BaseLogic(mapper, config), IItemsLogic
{
    private readonly IItemsDataService _itemsDataService = itemsDataService;

    public async Task<List<ItemDto>> GetItemsAsync()
    {
        var items = await _itemsDataService.GetItemsAsync();

        if (items == null) return [];

        return [.. items.Select(_mapper.Map<ItemDto>)];
    }
}
