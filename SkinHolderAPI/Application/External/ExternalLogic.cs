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

public class ExternalLogic(ILogLogic logLogic, IConfiguration configuration, IExternalDataService externalDataService, ILogger<ExternalLogic> logger) : IExternalLogic
{
    private const string FaceitPublicApi = "https://open.faceit.com/data/v4";
    private const string FaceitPrivateApi = "https://api.faceit.com";

    #region extension
    public async Task<string> GetPlayerInfoAsync(string playerId)
    {
        logger.LogInformation("GetPlayerInfoAsync llamado para playerId={PlayerId}", playerId);
        await logLogic.AddLogAsync(LogBuilder.BuildLoggerDto($"External call on GetPlayerInfo for player: {playerId}", LogType.Info, LogPlace.Api, 1));

        try
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);

            var response = await httpClient.GetAsync($"https://cswatch.in/api/players/{playerId}");

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("GetPlayerInfoAsync: respuesta no exitosa ({StatusCode}) para playerId={PlayerId}", (int)response.StatusCode, playerId);
                return string.Empty;
            }

            var result = await response.Content.ReadAsStringAsync();
            logger.LogInformation("GetPlayerInfoAsync completado para playerId={PlayerId}", playerId);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error en GetPlayerInfoAsync para playerId={PlayerId}", playerId);
            await logLogic.AddLogAsync(LogBuilder.BuildLoggerDto($"Error calling external API for player {playerId}: {ex.Message}", LogType.Error, LogPlace.Api, 1));
            return string.Empty;
        }
    }

    public async Task<(string, string)> GetFaceitPlayerDataAsync(string steamId)
    {
        logger.LogInformation("GetFaceitPlayerDataAsync llamado para steamId={SteamId}", steamId);

        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration["Faceit:ApiKey"]}");

            var url = $"{FaceitPublicApi}/players?game=cs2&game_player_id={steamId}";

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("GetFaceitPlayerDataAsync: respuesta no exitosa ({StatusCode}) para steamId={SteamId}", (int)response.StatusCode, steamId);
                return (string.Empty, string.Empty);
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var playerId = string.Empty;

            try
            {
                var jsonDocument = System.Text.Json.JsonDocument.Parse(jsonResponse);

                if (jsonDocument.RootElement.TryGetProperty("player_id", out var playerIdElement)) playerId = playerIdElement.GetString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error parseando datos de Faceit para steamId={SteamId}", steamId);
                await logLogic.AddLogAsync(LogBuilder.BuildLoggerDto($"Error parsing Faceit player data for Steam ID {steamId}: {ex.Message}", LogType.Error, LogPlace.Api, 1));
            }

            logger.LogInformation("GetFaceitPlayerDataAsync completado para steamId={SteamId}, playerId={PlayerId}", steamId, playerId);

            return (jsonResponse, playerId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error en GetFaceitPlayerDataAsync para steamId={SteamId}", steamId);
            return (string.Empty, string.Empty);
        }
    }

    public async Task<string> GetFaceitPlayerStatsAsync(string playerId, string game)
    {
        logger.LogInformation("GetFaceitPlayerStatsAsync llamado para playerId={PlayerId}, game={Game}", playerId, game);

        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration["Faceit:ApiKey"]}");

            var url = $"{FaceitPrivateApi}/stats/v1/stats/users/{playerId}/games/{game}";

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                logger.LogWarning("GetFaceitPlayerStatsAsync: respuesta no exitosa ({StatusCode}) para playerId={PlayerId}", (int)response.StatusCode, playerId);
            else
                logger.LogInformation("GetFaceitPlayerStatsAsync completado para playerId={PlayerId}", playerId);

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error en GetFaceitPlayerStatsAsync para playerId={PlayerId}, game={Game}", playerId, game);
            return string.Empty;
        }
    }

    public async Task<string> GetFaceitBansAsync(string playerId)
    {
        logger.LogInformation("GetFaceitBansAsync llamado para playerId={PlayerId}", playerId);

        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration["Faceit:ApiKey"]}");

            var url = $"{FaceitPublicApi}/players/{playerId}/bans";

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode) { logger.LogWarning("GetFaceitBansAsync: respuesta no exitosa ({StatusCode}) para playerId={PlayerId}", (int)response.StatusCode, playerId); }
            else { logger.LogInformation("GetFaceitBansAsync completado para playerId={PlayerId}", playerId); }

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error en GetFaceitBansAsync para playerId={PlayerId}", playerId);
            return string.Empty;
        }
    }

    public async Task<ExtensionUsageDto?> GetExtensionUsageAsync()
    {
        logger.LogInformation("GetExtensionUsageAsync llamado");

        try
        {
            var logs = await externalDataService.GetExtensionUsageAsync();

            if (logs == null || logs.Count == 0)
            {
                logger.LogWarning("GetExtensionUsageAsync: no se encontraron logs de uso");
                return null;
            }

            var now = DateTime.Now;
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
                RequestsGrowthRateLastMonth = twoMonthCount == 0 ? 0 : Math.Round((oneMonthCount - twoMonthCount) / (double)twoMonthCount * 100, 1),
                MaxRequestsInADay3m = maxRequestsInADay,
                LastUpdated = now
            };

            logger.LogInformation("GetExtensionUsageAsync completado: totalRequests3m={Total3m}, totalRequests1m={Total1m}", totalRequests3m, oneMonthCount);

            return extensionUsageDto;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error en GetExtensionUsageAsync");
            throw;
        }
    }
    #endregion

    #region sh
    public async Task<string> GetGamerPayPricesAsync()
    {
        logger.LogInformation("GetGamerPayPricesAsync llamado");

        try
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);

            var response = await httpClient.GetAsync($"https://api.gamerpay.gg/prices");

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("GetGamerPayPricesAsync: respuesta no exitosa ({StatusCode})", (int)response.StatusCode);
                return string.Empty;
            }

            var jsonString = await response.Content.ReadAsStringAsync();

            var fullData = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(jsonString);

            var filteredData = fullData.EnumerateArray()
                .Select(item => new { item = item.GetProperty("item").GetString(), price = item.GetProperty("price").GetDecimal() })
                .ToList();

            logger.LogInformation("GetGamerPayPricesAsync completado: {Count} items obtenidos", filteredData.Count);

            return System.Text.Json.JsonSerializer.Serialize(filteredData);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error en GetGamerPayPricesAsync");
            return string.Empty;
        }
    }
    #endregion
}