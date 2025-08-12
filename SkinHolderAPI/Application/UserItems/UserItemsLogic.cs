using AutoMapper;
using SkinHolderAPI.Application.Shared;
using SkinHolderAPI.DataService.UserItems;
using SkinHolderAPI.DTOs.UserItemsDto;

namespace SkinHolderAPI.Application.UserItems;

public interface IUserItemsLogic
{
    Task<List<UserItemDto>> GetUserItemsAsync(int userId);
}

public class UserItemsLogic(IUserItemsDataService userItemsDataService, IMapper mapper, IConfiguration config) : BaseLogic(mapper, config), IUserItemsLogic
{
    private readonly IUserItemsDataService _userItemsDataService = userItemsDataService;

    public async Task<List<UserItemDto>> GetUserItemsAsync(int userId)
    {
        var userItems = await _userItemsDataService.GetUserItemsAsync(userId);

        if (userItems == null || userItems.Count == 0) return [];

        var userItemsDto = _mapper.Map<List<UserItemDto>>(userItems);

        userItemsDto.Sort((x, y) => string.Compare(x.ItemName, y.ItemName, StringComparison.OrdinalIgnoreCase));

        return userItemsDto;
    }
}
