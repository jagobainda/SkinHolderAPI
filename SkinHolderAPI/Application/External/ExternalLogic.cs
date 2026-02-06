using SkinHolderAPI.Application.Loggers;
using SkinHolderAPI.DataService.External;
using SkinHolderAPI.DTOs.External;
using SkinHolderAPI.Utils;

namespace SkinHolderAPI.Application.External;

public interface IExternalLogic
{
    Task<string> GetPlayerInfoAsync(string playerId);
    Task<(string, string)> GetFaceitPlayerDataAsync(string steamId);
    Task<string> GetFaceitPlayerStatsAsync(string playerId, string game);
    Task<string> GetFaceitBansAsync(string playerId);
    Task<ExtensionUsageDto?> GetExtensionUsageAsync();
    Task<string> GetGamerPayPricesAsync();
}

public class ExternalLogic(IConfiguration config, ILogLogic logLogic, IConfiguration configuration, IExternalDataService externalDataService) : IExternalLogic
{
    public readonly IConfiguration _config = config;
    public readonly ILogLogic _logLogic = logLogic;
    public readonly IConfiguration _configuration = configuration;
    public readonly IExternalDataService _externalDataService = externalDataService;
    private const string FaceitPublicApi = "https://open.faceit.com/data/v4";
    private const string FaceitPrivateApi = "https://api.faceit.com";

    #region extension
    public async Task<string> GetPlayerInfoAsync(string playerId)
    {
        await _logLogic.AddLogAsync(LogBuilder.BuildLoggerDto($"External call on GetPlayerInfo for player: {playerId}", LogType.Info, LogPlace.Api, 1));

        try
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);

            var response = await httpClient.GetAsync($"https://cswatch.in/api/players/{playerId}");

            if (!response.IsSuccessStatusCode) return string.Empty;

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            await _logLogic.AddLogAsync(LogBuilder.BuildLoggerDto($"Error calling external API for player {playerId}: {ex.Message}", LogType.Error, LogPlace.Api, 1));
            return string.Empty;
        }
    }

    public async Task<(string, string)> GetFaceitPlayerDataAsync(string steamId)
    {
        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuration["Faceit:ApiKey"]}");

            var url = $"{FaceitPublicApi}/players?game=cs2&game_player_id={steamId}";

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode) return (string.Empty, string.Empty);

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var playerId = string.Empty;

            try
            {
                var jsonDocument = System.Text.Json.JsonDocument.Parse(jsonResponse);

                if (jsonDocument.RootElement.TryGetProperty("player_id", out var playerIdElement)) playerId = playerIdElement.GetString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                await _logLogic.AddLogAsync(LogBuilder.BuildLoggerDto($"Error parsing Faceit player data for Steam ID {steamId}: {ex.Message}", LogType.Error, LogPlace.Api, 1));
            }

            return (jsonResponse, playerId);
        }
        catch
        {
            return (string.Empty, string.Empty);
        }
    }

    public async Task<string> GetFaceitPlayerStatsAsync(string playerId, string game)
    {
        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuration["Faceit:ApiKey"]}");

            var url = $"{FaceitPrivateApi}/stats/v1/stats/users/{playerId}/games/{game}";

            var response = await httpClient.GetAsync(url);

            return await response.Content.ReadAsStringAsync();
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task<string> GetFaceitBansAsync(string playerId)
    {
        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuration["Faceit:ApiKey"]}");

            var url = $"{FaceitPublicApi}/players/{playerId}/bans";

            var response = await httpClient.GetAsync(url);

            return await response.Content.ReadAsStringAsync();
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task<ExtensionUsageDto?> GetExtensionUsageAsync()
    {
        var logs = await _externalDataService.GetExtensionUsageAsync();

        if (logs == null || logs.Count == 0) return null;

        var now = DateTime.UtcNow;
        var threeMonthsAgo = now.AddMonths(-3);
        var twoMonthsAgo = now.AddMonths(-2);
        var oneMonthAgo = now.AddMonths(-1);

        var processedData = logs
            .Where(l => l.LogDateTime >= threeMonthsAgo && l.LogDescription.Contains("External call on GetPlayerInfo"))
            .GroupBy(l => l.LogDateTime.Date)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count(),
                IsInOneMonth = g.Key >= oneMonthAgo,
                IsInTwoMonths = g.Key >= twoMonthsAgo && g.Key < oneMonthAgo
            })
            .ToList();

        var totalRequests3m = processedData.Sum(d => d.Count);
        var oneMonthCount = processedData.Where(d => d.IsInOneMonth).Sum(d => d.Count);
        var twoMonthCount = processedData.Where(d => d.IsInTwoMonths).Sum(d => d.Count);
        var maxRequestsInADay = processedData.Count > 0 ? processedData.Max(d => d.Count) : 0;

        var daysCount3m = (now - threeMonthsAgo).TotalDays;
        var daysCount1m = (now - oneMonthAgo).TotalDays;

        var extensionUsageDto = new ExtensionUsageDto()
        {
            AvgDailyRequests3m = (int)Math.Ceiling(totalRequests3m / daysCount3m),
            TotalRequests3m = totalRequests3m,
            AvgDailyRequests1m = (int)Math.Ceiling(oneMonthCount / daysCount1m),
            TotalRequests1m = oneMonthCount,
            RequestsGrowthRateLastMonth = totalRequests3m == 0 ? 0 : (oneMonthCount - twoMonthCount) / (double)totalRequests3m * 100,
            MaxRequestsInADay3m = maxRequestsInADay,
            LastUpdatedUtc = now
        };

        return extensionUsageDto;
    }
    #endregion

    #region sh
    public async Task<string> GetGamerPayPricesAsync()
    {
        try
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);

            var response = await httpClient.GetAsync($"https://api.gamerpay.gg/prices");

            if (!response.IsSuccessStatusCode) return string.Empty;

            var jsonString = await response.Content.ReadAsStringAsync();

            var fullData = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(jsonString);

            var filteredData = fullData.EnumerateArray()
                .Select(item => new { item = item.GetProperty("item").GetString(), price = item.GetProperty("price").GetDecimal() })
                .ToList();

            return System.Text.Json.JsonSerializer.Serialize(filteredData);
        }
        catch
        {
            return string.Empty;
        }
    }
    #endregion
}