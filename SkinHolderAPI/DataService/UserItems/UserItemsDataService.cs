using Microsoft.EntityFrameworkCore;
using SkinHolderAPI.DataService.Contexts;
using SkinHolderAPI.Models;

namespace SkinHolderAPI.DataService.UserItems;

public interface IUserItemsDataService
{
    Task<List<Useritem>?> GetUserItemsAsync(int userId);
}

public class UserItemsDataService(SkinHolderDbContext context) : IUserItemsDataService
{
    private readonly SkinHolderDbContext _context = context;

    public async Task<List<Useritem>?> GetUserItemsAsync(int userId)
    {
        return await _context.Useritems.Where(ui => ui.Userid == userId).ToListAsync();
    }
}
