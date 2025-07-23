using Microsoft.EntityFrameworkCore;
using SkinHolderAPI.DataService.Contexts;
using SkinHolderAPI.Models;

namespace SkinHolderAPI.DataService.Items;

public interface IItemsDataService
{
    Task<List<Item>?> GetItemsAsync();
}

public class ItemsDataService(SkinHolderDbContext context) : IItemsDataService
{
    private readonly SkinHolderDbContext _context = context;

    public async Task<List<Item>?> GetItemsAsync()
    {
        return await _context.Items.OrderBy(i => i.Nombre).ToListAsync();
    }
}
