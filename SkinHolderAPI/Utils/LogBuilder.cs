using SkinHolderAPI.Application.Loggers;
using SkinHolderAPI.DTOs.Loggers;

namespace SkinHolderAPI.Utils;

public static class LogBuilder
{
    public static LoggerDto BuildLoggerDto(string description, LogType logType, LogPlace logPlace, int userId)
    {
        return new LoggerDto
        {
            LogDescription = description,
            LogTypeId = logType,
            LogPlaceId = logPlace,
            UserId = userId
        };
    }
}
