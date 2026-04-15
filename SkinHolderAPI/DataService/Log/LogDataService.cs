using SkinHolderAPI.DataService.Contexts;
using SkinHolderAPI.Models.Logs;

namespace SkinHolderAPI.DataService.Log;

public interface ILogDataService
{
    Task<bool> AddLogAsync(Logger logger);
    Task<bool> DeleteOldLogsAsync(DateTime cutoffDate);
}

public class LogDataService(SkinHolderLogDbContext dbContext, ILogger<LogDataService> logger) : ILogDataService
{
    private readonly SkinHolderLogDbContext _dbContext = dbContext;
    private readonly ILogger<LogDataService> _logger = logger;

    public async Task<bool> AddLogAsync(Logger logger)
    {
        try
        {
            await _dbContext.Loggers.AddAsync(logger);

            await _dbContext.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en AddLogAsync al persistir log en base de datos");
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en DeleteOldLogsAsync para cutoffDate={CutoffDate}", cutoffDate);
            return false;
        }
    }
}
