using Microsoft.EntityFrameworkCore;
using SkinHolderAPI.Application.Log;
using SkinHolderAPI.DataService.Contexts;
using SkinHolderAPI.Models;
using SkinHolderAPI.Utils;

namespace SkinHolderAPI.DataService.ItemPrecio;

public interface IItemPrecioDataService
{
    Task<List<Itemprecio>?> GetItemPreciosAsync(long registroId);
    Task<bool> CreateItemPreciosAsync(List<Itemprecio> itemPrecios);
    Task<bool> DeleteItemPreciosAsync(long registroId);
}

public class ItemPrecioDataService(SkinHolderDbContext context, ILogLogic logLogic) : IItemPrecioDataService
{
    private readonly SkinHolderDbContext _context = context;
    private readonly ILogLogic _logLogic = logLogic;

    private async Task<T?> SafeExecuteAsync<T>(Func<Task<T>> action, string methodName, int userId = 0)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            await _logLogic.AddLogAsync(LogBuilder.BuildLoggerDto($"Exception in {methodName}: {ex.Message}", LogType.Error, LogPlace.Api, userId));

            return default;
        }
    }

    public async Task<List<Itemprecio>?> GetItemPreciosAsync(long registroId)
    {
        return await SafeExecuteAsync(
            async () => await _context.Itemprecios
                                    .Where(ip => ip.Registroid == registroId)
                                    .Include(ip => ip.Useritem)
                                    .ToListAsync(),
            nameof(GetItemPreciosAsync)
        );
    }

    public async Task<bool> CreateItemPreciosAsync(List<Itemprecio> itemPrecios)
    {
        if (itemPrecios == null || itemPrecios.Count == 0) return false;

        return await SafeExecuteAsync(async () =>
        {
            await _context.Itemprecios.AddRangeAsync(itemPrecios);
            await _context.SaveChangesAsync();
            return true;
        }, nameof(CreateItemPreciosAsync));
    }

    public async Task<bool> DeleteItemPreciosAsync(long registroId)
    {
        return await SafeExecuteAsync(async () =>
        {
            var itemPrecios = await _context.Itemprecios
                                          .Where(ip => ip.Registroid == registroId)
                                          .ToListAsync();

            if (itemPrecios.Count == 0) return false;

            _context.Itemprecios.RemoveRange(itemPrecios);
            await _context.SaveChangesAsync();
            return true;
        }, nameof(DeleteItemPreciosAsync));
    }
}