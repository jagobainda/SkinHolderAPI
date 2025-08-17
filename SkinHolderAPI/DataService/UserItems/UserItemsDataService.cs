using Microsoft.EntityFrameworkCore;
using SkinHolderAPI.Application.Log;
using SkinHolderAPI.DataService.Contexts;
using SkinHolderAPI.Models;
using SkinHolderAPI.Utils;

namespace SkinHolderAPI.DataService.UserItems;

public interface IUserItemsDataService 
{ 
    Task<List<Useritem>?> GetUserItemsAsync(int userId); 
    Task<bool> CreateUserItemAsync(Useritem userItem); 
    Task<bool> UpdateUserItemAsync(long userItemId, int cantidad); 
    Task<bool> DeleteUserItemAsync(long userItemId); 
}

public class UserItemsDataService : IUserItemsDataService
{
    private readonly SkinHolderDbContext _context;
    private readonly ILogLogic _logLogic;

    public UserItemsDataService(SkinHolderDbContext context, ILogLogic logLogic)
    {
        _context = context;
        _logLogic = logLogic;
    }

    private async Task<T?> SafeExecuteAsync<T>(Func<Task<T>> action, string methodName, int userId = 0)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            await _logLogic.AddLogAsync(
                LogBuilder.BuildLoggerDto(
                    $"Exception in {methodName}: {ex.Message}",
                    LogType.Error,
                    LogPlace.Api,
                    userId
                )
            );
            return default;
        }
    }

    public async Task<List<Useritem>?> GetUserItemsAsync(int userId)
    {
        return await SafeExecuteAsync(
            async () => await _context.Useritems
                                      .Where(ui => ui.Userid == userId)
                                      .Include(ui => ui.Item)
                                      .ToListAsync(),
            nameof(GetUserItemsAsync),
            userId
        );
    }

    public async Task<bool> CreateUserItemAsync(Useritem userItem)
    {
        if (userItem == null) return false;

        return await SafeExecuteAsync(async () =>
        {
            await _context.Useritems.AddAsync(userItem);
            await _context.SaveChangesAsync();
            return true;
        }, nameof(CreateUserItemAsync), userItem.Userid);
    }

    public async Task<bool> UpdateUserItemAsync(long userItemId, int cantidad)
    {
        return await SafeExecuteAsync(async () =>
        {
            var userItem = await _context.Useritems.FirstOrDefaultAsync(ui => ui.Useritemid == userItemId);

            if (userItem == null) return false;

            userItem.Cantidad = cantidad;
            _context.Useritems.Update(userItem);

            await _context.SaveChangesAsync();
            return true;
        }, nameof(UpdateUserItemAsync));
    }

    public async Task<bool> DeleteUserItemAsync(long userItemId)
    {
        return await SafeExecuteAsync(async () =>
        {
            var userItem = await _context.Useritems.FirstOrDefaultAsync(ui => ui.Useritemid == userItemId);

            if (userItem == null) return false;

            _context.Useritems.Remove(userItem);
            await _context.SaveChangesAsync();
            return true;
        }, nameof(DeleteUserItemAsync));
    }
}
