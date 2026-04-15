using Microsoft.EntityFrameworkCore;
using SkinHolderAPI.DataService.Contexts;
using SkinHolderAPI.Models.Logs;

namespace SkinHolderAPI.DataService.External;

public interface IExternalDataService
{
    Task<List<Logger>> GetExtensionUsageAsync();
}

public class ExternalDataService(SkinHolderLogDbContext context, ILogger<ExternalDataService> logger) : IExternalDataService
{
    private readonly SkinHolderLogDbContext _context = context;
    private readonly ILogger<ExternalDataService> _logger = logger;

    public async Task<List<Logger>> GetExtensionUsageAsync()
    {
        try
        {
            return await _context.Loggers.Where(l => l.LogPlaceId == 3).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetExtensionUsageAsync al obtener logs de uso de extensión");
            return [];
        }
    }
}
