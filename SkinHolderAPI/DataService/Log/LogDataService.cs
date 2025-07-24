using SkinHolderAPI.DataService.Contexts;
using SkinHolderAPI.Models.Logs;

namespace SkinHolderAPI.DataService.Log;

public interface ILogDataService
{
    Task<bool> AddLogAsync(Logger logger);
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
}
