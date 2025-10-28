using SkinHolderAPI.DataService.Log;
using SkinHolderAPI.DTOs.Loggers;
using SkinHolderAPI.Models.Logs;

namespace SkinHolderAPI.Application.Loggers;

public interface ILogLogic
{
    Task<bool> AddLogAsync(LoggerDto logger);
}

public class LogLogic(ILogDataService logDataService) : ILogLogic
{
    private readonly ILogDataService _logDataService = logDataService;

    public async Task<bool> AddLogAsync(LoggerDto loggerDto)
    {
        if (loggerDto == null) return false;

        var logger = new Logger
        {
            LoggerId = 0,
            LogDescription = loggerDto.LogDescription,
            LogDateTime = DateTime.UtcNow,
            LogTypeId = loggerDto.LogTypeId.GetHashCode(),
            LogPlaceId = loggerDto.LogPlaceId.GetHashCode(),
            UserId = loggerDto.UserId
        };

         return await _logDataService.AddLogAsync(logger);
    }
}

public enum LogType
{
    Info = 1,
    Warning = 2,
    Error = 3
}

public enum LogPlace
{
    Desktop = 1,
    Android = 2,
    Api = 3
}