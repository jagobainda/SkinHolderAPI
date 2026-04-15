using MapsterMapper;
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

public class ItemPrecioLogic(IItemPrecioDataService itemPrecioDataService, IMapper mapper, IConfiguration config, ILogger<ItemPrecioLogic> logger) : BaseLogic(mapper, config, logger), IItemPrecioLogic
{
    private readonly IItemPrecioDataService _itemPrecioDataService = itemPrecioDataService;

    public async Task<List<ItemPrecioDto>> GetItemPreciosAsync(long registroId)
    {
        _logger.LogInformation("GetItemPreciosAsync llamado para registroId={RegistroId}", registroId);

        try
        {
            var itemPrecios = await _itemPrecioDataService.GetItemPreciosAsync(registroId);

            if (itemPrecios == null || itemPrecios.Count == 0)
            {
                _logger.LogWarning("GetItemPreciosAsync no devolvió precios para registroId={RegistroId}", registroId);
                return [];
            }

            var result = itemPrecios.Select(ip => _mapper.Map<ItemPrecioDto>(ip)).ToList();

            _logger.LogInformation("GetItemPreciosAsync completado: {Count} precios para registroId={RegistroId}", result.Count, registroId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetItemPreciosAsync para registroId={RegistroId}", registroId);
            throw;
        }
    }

    public async Task<bool> CreateItemPreciosAsync(List<ItemPrecioDto> itemPreciosDto)
    {
        if (itemPreciosDto == null || itemPreciosDto.Count == 0)
        {
            _logger.LogWarning("CreateItemPreciosAsync rechazado: lista vacía o nula");
            return false;
        }

        _logger.LogInformation("CreateItemPreciosAsync llamado con {Count} precios", itemPreciosDto.Count);

        try
        {
            var itemPrecios = itemPreciosDto.Select(ip => _mapper.Map<Itemprecio>(ip)).ToList();
            var result = await _itemPrecioDataService.CreateItemPreciosAsync(itemPrecios);

            if (!result)
                _logger.LogWarning("CreateItemPreciosAsync falló al persistir {Count} precios", itemPreciosDto.Count);
            else
                _logger.LogInformation("CreateItemPreciosAsync completado: {Count} precios creados", itemPreciosDto.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en CreateItemPreciosAsync con {Count} precios", itemPreciosDto.Count);
            throw;
        }
    }

    public async Task<bool> DeleteItemPreciosAsync(long registroId)
    {
        _logger.LogInformation("DeleteItemPreciosAsync llamado para registroId={RegistroId}", registroId);

        try
        {
            var result = await _itemPrecioDataService.DeleteItemPreciosAsync(registroId);

            if (!result)
                _logger.LogWarning("DeleteItemPreciosAsync falló o no encontró precios para registroId={RegistroId}", registroId);
            else
                _logger.LogInformation("DeleteItemPreciosAsync completado para registroId={RegistroId}", registroId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en DeleteItemPreciosAsync para registroId={RegistroId}", registroId);
            throw;
        }
    }
}
