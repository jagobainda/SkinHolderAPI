using MapsterMapper;
using SkinHolderAPI.Application.Shared;
using SkinHolderAPI.DataService.UserItems;
using SkinHolderAPI.DTOs.UserItemsDto;
using SkinHolderAPI.Models;

namespace SkinHolderAPI.Application.UserItems;

public interface IUserItemsLogic
{
    Task<List<UserItemDto>> GetUserItemsAsync(int userId);
    Task<bool> CreateUserItemAsync(UserItemDto userItemDto);
    Task<bool> UpdateUserItemAsync(UserItemDto userItemDto);
}

public class UserItemsLogic(IUserItemsDataService userItemsDataService, IMapper mapper, IConfiguration config, ILogger<UserItemsLogic> logger) : BaseLogic(mapper, config, logger), IUserItemsLogic
{
    private readonly IUserItemsDataService _userItemsDataService = userItemsDataService;

    public async Task<List<UserItemDto>> GetUserItemsAsync(int userId)
    {
        _logger.LogInformation("GetUserItemsAsync llamado para userId={UserId}", userId);

        try
        {
            var userItems = await _userItemsDataService.GetUserItemsAsync(userId);

            if (userItems == null || userItems.Count == 0)
            {
                _logger.LogWarning("GetUserItemsAsync no devolvió items para userId={UserId}", userId);
                return [];
            }

            var userItemsDto = _mapper.Map<List<UserItemDto>>(userItems);
            userItemsDto.Sort((x, y) => string.Compare(x.ItemName, y.ItemName, StringComparison.OrdinalIgnoreCase));

            _logger.LogInformation("GetUserItemsAsync completado: {Count} items para userId={UserId}", userItemsDto.Count, userId);

            return userItemsDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetUserItemsAsync para userId={UserId}", userId);
            throw;
        }
    }

    public async Task<bool> CreateUserItemAsync(UserItemDto userItemDto)
    {
        if (userItemDto == null || userItemDto.Cantidad <= 0)
        {
            _logger.LogWarning("CreateUserItemAsync rechazado: datos inválidos o cantidad <= 0");
            return false;
        }

        _logger.LogInformation("CreateUserItemAsync llamado para item={ItemName}, userId={UserId}, cantidad={Cantidad}", userItemDto.ItemName, userItemDto.Userid, userItemDto.Cantidad);

        try
        {
            var userItem = _mapper.Map<Useritem>(userItemDto);
            userItem.Useritemid = 0;
            userItem.Preciomediocompra = 0m;

            var result = await _userItemsDataService.CreateUserItemAsync(userItem);

            if (!result)
                _logger.LogWarning("CreateUserItemAsync falló al persistir el item para userId={UserId}", userItemDto.Userid);
            else
                _logger.LogInformation("CreateUserItemAsync completado para userId={UserId}", userItemDto.Userid);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en CreateUserItemAsync para userId={UserId}", userItemDto.Userid);
            throw;
        }
    }

    public async Task<bool> UpdateUserItemAsync(UserItemDto userItemDto)
    {
        _logger.LogInformation("UpdateUserItemAsync llamado para userItemId={UserItemId}, cantidad={Cantidad}", userItemDto.Useritemid, userItemDto.Cantidad);

        try
        {
            bool result;

            if (userItemDto.Cantidad > 0)
            {
                result = await _userItemsDataService.UpdateUserItemAsync(userItemDto.Useritemid, userItemDto.Cantidad);
                if (!result)
                    _logger.LogWarning("UpdateUserItemAsync falló al actualizar userItemId={UserItemId}", userItemDto.Useritemid);
                else
                    _logger.LogInformation("UpdateUserItemAsync completado: userItemId={UserItemId} actualizado a cantidad={Cantidad}", userItemDto.Useritemid, userItemDto.Cantidad);
            }
            else
            {
                _logger.LogInformation("UpdateUserItemAsync: cantidad=0, eliminando userItemId={UserItemId}", userItemDto.Useritemid);
                result = await _userItemsDataService.DeleteUserItemAsync(userItemDto.Useritemid);
                if (!result)
                    _logger.LogWarning("UpdateUserItemAsync falló al eliminar userItemId={UserItemId}", userItemDto.Useritemid);
                else
                    _logger.LogInformation("UpdateUserItemAsync: userItemId={UserItemId} eliminado correctamente", userItemDto.Useritemid);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en UpdateUserItemAsync para userItemId={UserItemId}", userItemDto.Useritemid);
            throw;
        }
    }
}
