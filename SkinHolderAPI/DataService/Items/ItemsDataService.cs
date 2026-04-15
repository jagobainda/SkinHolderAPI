using Microsoft.EntityFrameworkCore;
using SkinHolderAPI.DataService.Contexts;
using SkinHolderAPI.Models;

namespace SkinHolderAPI.DataService.Items;

public interface IItemsDataService
{
    Task<List<Item>?> GetItemsAsync();
}

public class ItemsDataService(SkinHolderDbContext context, ILogger<ItemsDataService> logger) : IItemsDataService
{
    private readonly SkinHolderDbContext _context = context;
    private readonly ILogger<ItemsDataService> _logger = logger;

    public async Task<List<Item>?> GetItemsAsync()
    {
        try
        {
            return await _context.Items.OrderBy(i => i.Nombre).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetItemsAsync al obtener items de la base de datos");
            return null;
        }
    }
}
