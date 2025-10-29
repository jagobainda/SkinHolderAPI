using SkinHolderAPI.DataService.Contexts;
using SkinHolderAPI.Models.Logs;

namespace SkinHolderAPI.DataService.Log;

public interface ILogDataService
{
    Task<bool> AddLogAsync(Logger logger);
    Task<bool> DeleteOldLogsAsync(DateTime cutoffDate);
}

public class LogDataService(SkinHolderLogDbContext dbContext) : ILogDataService
{
    private readonly SkinHolderLogDbContext _dbContext = dbContext;

    public async Task<bool> AddLogAsync(Logger logger)
    {
        try
        {
            await _dbContext.Loggers.AddAsync(logger);

            await _dbContext.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteOldLogsAsync(DateTime cutoffDate)
    {
        try
        {
            var oldLogs = _dbContext.Loggers.Where(log => log.LogDateTime < cutoffDate);

            _dbContext.Loggers.RemoveRange(oldLogs);

            await _dbContext.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }
}
