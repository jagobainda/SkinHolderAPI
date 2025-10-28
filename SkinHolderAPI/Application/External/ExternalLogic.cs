using SkinHolderAPI.Application.Loggers;
using SkinHolderAPI.Utils;

namespace SkinHolderAPI.Application.External;

public interface IExternalLogic
{
    Task<string> GetPlayerInfo(string playerId);
}

public class ExternalLogic(IConfiguration config, ILogLogic logLogic) : IExternalLogic
{
    public readonly IConfiguration _config = config;
    public readonly ILogLogic _logLogic = logLogic;

    public async Task<string> GetPlayerInfo(string playerId)
    {
        _ = Task.Run(async () => await _logLogic.AddLogAsync(LogBuilder.BuildLoggerDto($"External call on GetPlayerInfo", LogType.Info, LogPlace.Api, 1)));

        return "Player Info";
    }
}
