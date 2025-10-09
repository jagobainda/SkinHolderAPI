using AutoMapper;
using SkinHolderAPI.Application.Shared;
using SkinHolderAPI.DataService.ItemPrecio;
using SkinHolderAPI.DTOs.ItemPrecio;
using SkinHolderAPI.Models;

namespace SkinHolderAPI.Application.ItemPrecio;

public interface IItemPrecioLogic
{
    Task<List<ItemPrecioDto>> GetItemPreciosAsync(long registroId);
    Task<bool> CreateItemPreciosAsync(List<ItemPrecioDto> itemPreciosDto);
    Task<bool> DeleteItemPreciosAsync(long registroId);
}

public class ItemPrecioLogic(IItemPrecioDataService itemPrecioDataService, IMapper mapper, IConfiguration config) : BaseLogic(mapper, config), IItemPrecioLogic
{
    private readonly IItemPrecioDataService _itemPrecioDataService = itemPrecioDataService;

    public async Task<List<ItemPrecioDto>> GetItemPreciosAsync(long registroId)
    {
        var itemPrecios = await _itemPrecioDataService.GetItemPreciosAsync(registroId);

        if (itemPrecios == null) return [];

        return [.. itemPrecios.Select(_mapper.Map<ItemPrecioDto>)];
    }

    public async Task<bool> CreateItemPreciosAsync(List<ItemPrecioDto> itemPreciosDto)
    {
        if (itemPreciosDto == null || itemPreciosDto.Count == 0) return false;

        var itemPrecios = itemPreciosDto.Select(_mapper.Map<Itemprecio>).ToList();

        return await _itemPrecioDataService.CreateItemPreciosAsync(itemPrecios);
    }

    public async Task<bool> DeleteItemPreciosAsync(long registroId)
    {
        return await _itemPrecioDataService.DeleteItemPreciosAsync(registroId);
    }
}
