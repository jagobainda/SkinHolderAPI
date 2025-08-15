using Microsoft.EntityFrameworkCore;
using SkinHolderAPI.DataService.Contexts;
using SkinHolderAPI.Models;

namespace SkinHolderAPI.DataService.UserItems;

public interface IUserItemsDataService
{
    Task<List<Useritem>?> GetUserItemsAsync(int userId);
    Task<bool> CreateUserItemAsync(Useritem userItem);
    Task<bool> UpdateUserItemAsync(long userItemId, int cantidad);
    Task<bool> DeleteUserItemAsync(long userItemId);
}

public class UserItemsDataService(SkinHolderDbContext context) : IUserItemsDataService
{
    private readonly SkinHolderDbContext _context = context;

    public async Task<List<Useritem>?> GetUserItemsAsync(int userId)
    {
        return await _context.Useritems.Where(ui => ui.Userid == userId).Include(ui => ui.Item).ToListAsync();
    }

    public async Task<bool> CreateUserItemAsync(Useritem userItem)
    {
        if (userItem == null) return false;

        _context.Useritems.Add(userItem);

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateUserItemAsync(long userItemId, int cantidad)
    {
        var userItem = await _context.Useritems.FirstOrDefaultAsync(ui => ui.Useritemid == userItemId);

        if (userItem == null) return false;

        userItem.Cantidad = cantidad;

        _context.Useritems.Update(userItem);

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteUserItemAsync(long userItemId)
    {
        var userItem = await _context.Useritems.FirstOrDefaultAsync(ui => ui.Useritemid == userItemId);

        if (userItem == null) return false;

        _context.Useritems.Remove(userItem);

        return await _context.SaveChangesAsync() > 0;
    }
}
