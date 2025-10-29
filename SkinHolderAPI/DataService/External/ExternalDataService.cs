using Microsoft.EntityFrameworkCore;
using SkinHolderAPI.Application.Loggers;
using SkinHolderAPI.DataService.Contexts;
using SkinHolderAPI.Models.Logs;
using SkinHolderAPI.Utils;

namespace SkinHolderAPI.DataService.External;

public interface IExternalDataService
{
    Task<List<Logger>> GetExtensionUsageAsync();
}

public class ExternalDataService(SkinHolderLogDbContext context, ILogLogic logLogic) : IExternalDataService
{
    private readonly SkinHolderLogDbContext _context = context;
    private readonly ILogLogic _logLogic = logLogic;

    public async Task<List<Logger>> GetExtensionUsageAsync()
    {
        try
        {
            var logs = await _context.Loggers.Where(l => l.LogPlaceId == 3).ToListAsync();

            await _logLogic.AddLogAsync(LogBuilder.BuildLoggerDto($"Fetched {logs.Count} extension usage logs", LogType.Info, LogPlace.Api, 1));

            return logs;
        }
        catch
        {
            return [];
        }
    }
}
