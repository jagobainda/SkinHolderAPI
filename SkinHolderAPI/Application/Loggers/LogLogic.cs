using SkinHolderAPI.DataService.Log;
using SkinHolderAPI.DTOs.Loggers;
using SkinHolderAPI.Models.Logs;

namespace SkinHolderAPI.Application.Loggers;

public interface ILogLogic
{
    Task<bool> AddLogAsync(LoggerDto logger);
    Task<bool> DeleteOldLogsAsync(DateTime cutoffDate);
}

public class LogLogic(ILogDataService logDataService, ILogger<LogLogic> logger) : ILogLogic
{
    private readonly ILogDataService _logDataService = logDataService;
    private readonly ILogger<LogLogic> _logger = logger;

    public async Task<bool> AddLogAsync(LoggerDto loggerDto)
    {
        if (loggerDto == null)
        {
            _logger.LogWarning("AddLogAsync: loggerDto es null, ignorando");
            return false;
        }

        try
        {
            var logEntity = new Logger
            {
                LoggerId = 0,
                LogDescription = loggerDto.LogDescription,
                LogDateTime = DateTime.Now,
                LogTypeId = loggerDto.LogTypeId.GetHashCode(),
                LogPlaceId = loggerDto.LogPlaceId.GetHashCode(),
                UserId = loggerDto.UserId
            };

            return await _logDataService.AddLogAsync(logEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en AddLogAsync al persistir log: {Description}", loggerDto.LogDescription);
            return false;
        }
    }

    public async Task<bool> DeleteOldLogsAsync(DateTime cutoffDate)
    {
        _logger.LogInformation("DeleteOldLogsAsync llamado con cutoffDate={CutoffDate}", cutoffDate);

        try
        {
            var result = await _logDataService.DeleteOldLogsAsync(cutoffDate);

            if (!result)
                _logger.LogWarning("DeleteOldLogsAsync: la operación devolvió false para cutoffDate={CutoffDate}", cutoffDate);
            else
                _logger.LogInformation("DeleteOldLogsAsync completado para cutoffDate={CutoffDate}", cutoffDate);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en DeleteOldLogsAsync para cutoffDate={CutoffDate}", cutoffDate);
            return false;
        }
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