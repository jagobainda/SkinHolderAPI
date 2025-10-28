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
        await _logLogic.AddLogAsync(LogBuilder.BuildLoggerDto($"External call on GetPlayerInfo for player: {playerId}", LogType.Info, LogPlace.Api, 1));

        try
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);
            
            var response = await httpClient.GetAsync($"https://cswatch.in/api/players/{playerId}");
            
            if (!response.IsSuccessStatusCode) return string.Empty;
            
            var jsonResponse = await response.Content.ReadAsStringAsync();
            return jsonResponse;
        }
        catch (Exception ex)
        {
            await _logLogic.AddLogAsync(LogBuilder.BuildLoggerDto($"Error calling external API for player {playerId}: {ex.Message}", LogType.Error, LogPlace.Api, 1));
            return string.Empty;
        }
    }
}
