using SkinHolderAPI.Application.Log;

namespace SkinHolderAPI.DTOs.Loggers;

public class LoggerDto
{
    public string LogDescription { get; set; } = null!;

    public LogType LogTypeId { get; set; }

    public LogPlace LogPlaceId { get; set; }

    public int UserId { get; set; }
}
