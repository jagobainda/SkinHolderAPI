using Microsoft.EntityFrameworkCore;
using SkinHolderAPI.Application.Loggers;
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

public class UserItemsDataService(SkinHolderDbContext context, ILogLogic logLogic, ILogger<UserItemsDataService> logger) : IUserItemsDataService
{
    private readonly SkinHolderDbContext _context = context;
    private readonly ILogLogic _logLogic = logLogic;
    private readonly ILogger<UserItemsDataService> _logger = logger;

    private async Task<T?> SafeExecuteAsync<T>(Func<Task<T>> action, string methodName, int userId = 0)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            var fullMessage = ex.InnerException != null 
                ? $"{ex.Message} --> Inner: {ex.InnerException.Message}" 
                : ex.Message;
            _logger.LogError(ex, "Error en {MethodName} para userId={UserId}", methodName, userId);
            await _logLogic.AddLogAsync(
                LogBuilder.BuildLoggerDto(
                    $"Exception in {methodName}: {fullMessage}",
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
            var itemExists = await _context.Items.AnyAsync(i => i.Itemid == userItem.Itemid);
            _logger.LogWarning("CreateUserItemAsync PRE-INSERT: Itemid={Itemid}, Userid={Userid}, Cantidad={Cantidad}, ItemExists={ItemExists}",
                userItem.Itemid, userItem.Userid, userItem.Cantidad, itemExists);

            if (!itemExists)
            {
                _logger.LogError("CreateUserItemAsync: Itemid={Itemid} no existe en la tabla items", userItem.Itemid);
                return false;
            }

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
