using SkinHolderAPI.Application.Loggers;
using SkinHolderAPI.DTOs.ApiQuery;
using SkinHolderAPI.Utils;
using System.Text.Json;

namespace SkinHolderAPI.DataService.Apis;

public interface IApiQueryDataService
{
    Task<List<GamerPayItemDto>> GetGamerPayItemPricesAsync(List<string> itemNames);
    Task<List<CSFloatItemDto>> GetCSFloatItemPricesAsync(List<string> marketHashNames, string[] apiKeys);
}

public class ApiQueryDataService(ILogLogic logLogic) : IApiQueryDataService
{
    private readonly ILogLogic _logLogic = logLogic;

    public async Task<List<GamerPayItemDto>> GetGamerPayItemPricesAsync(List<string> itemNames)
    {
        var items = await FetchGamerPayDataAsync();
        return [.. items.Where(i => itemNames.Contains(i.Name))];
    }

    private async Task<GamerPayItemDto[]> FetchGamerPayDataAsync()
    {
        try
        {
            using var client = new HttpClient();

            var response = await client.GetAsync("https://api.gamerpay.gg/prices");
            response.EnsureSuccessStatusCode();
            var jsonContent = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(jsonContent);

            if (doc.RootElement.ValueKind == JsonValueKind.Array)
            {
                List<GamerPayItemDto> items = [];

                items.AddRange(from element in doc.RootElement.EnumerateArray()
                               let name = element.GetProperty("item").GetString()
                               let price = element.GetProperty("price").GetDouble()
                               select new GamerPayItemDto { Name = name, Price = price });

                return [.. items];
            }
        }
        catch (HttpRequestException ex)
        {
            await _logLogic.AddLogAsync(LogBuilder.BuildLoggerDto("HttpRequestException in method FetchGamerPayDataAsync(): " + ex.Message, LogType.Error, LogPlace.Api, 1));
        }
        catch (JsonException ex)
        {
            await _logLogic.AddLogAsync(LogBuilder.BuildLoggerDto("JsonException in method FetchGamerPayDataAsync(): " + ex.Message, LogType.Error, LogPlace.Api, 1));
        }
        catch (Exception ex)
        {
            await _logLogic.AddLogAsync(LogBuilder.BuildLoggerDto("Exception in method FetchGamerPayDataAsync(): " + ex.Message, LogType.Error, LogPlace.Api, 1));
        }

        return [];
    }

    public async Task<List<CSFloatItemDto>> GetCSFloatItemPricesAsync(List<string> marketHashNames, string[] apiKeys)
    {
        try
        {
            var usdToEur = await GetUsdToEur();
            if (usdToEur == 0.0)
            {
                await _logLogic.AddLogAsync(LogBuilder.BuildLoggerDto("GetUsdToEur returned 0.0", LogType.Warning, LogPlace.Api, 1));
                return [];
            }

            if (marketHashNames.Count == 0)
            {
                await _logLogic.AddLogAsync(LogBuilder.BuildLoggerDto("No marketHashNames provided", LogType.Info, LogPlace.Api, 1));
                return [];
            }

            using var client = new HttpClient();
            List<CSFloatItemDto> items = [];
            int currentKeyIndex = 0;

            foreach (var marketHashName in marketHashNames)
            {
                CSFloatItemDto? item = null;

                while (currentKeyIndex < apiKeys.Length)
                {
                    try
                    {
                        var url = $"https://csfloat.com/api/v1/listings?limit=1&sort_by=lowest_price&market_hash_name={marketHashName}";
                        var request = new HttpRequestMessage(HttpMethod.Get, url);
                        request.Headers.Add("Authorization", apiKeys[currentKeyIndex]);

                        var response = await client.SendAsync(request);

                        if ((int)response.StatusCode == 429)
                        {
                            await _logLogic.AddLogAsync(LogBuilder.BuildLoggerDto($"Rate limit (429) with API key index {currentKeyIndex} for '{marketHashName}'", LogType.Info, LogPlace.Api, 1));
                            currentKeyIndex++;
                            continue;
                        }

                        if (!response.IsSuccessStatusCode)
                        {
                            await _logLogic.AddLogAsync(LogBuilder.BuildLoggerDto($"Unsuccessful response ({(int)response.StatusCode}) for '{marketHashName}' with key index {currentKeyIndex}", LogType.Warning, LogPlace.Api, 1));
                            return [];
                        }

                        using var stream = await response.Content.ReadAsStreamAsync();
                        using var doc = await JsonDocument.ParseAsync(stream);

                        if (!doc.RootElement.TryGetProperty("data", out var dataElement) || dataElement.GetArrayLength() == 0)
                        {
                            await _logLogic.AddLogAsync(LogBuilder.BuildLoggerDto($"No data or invalid JSON for '{marketHashName}'", LogType.Warning, LogPlace.Api, 1));
                            return [];
                        }

                        var listing = dataElement[0];
                        var marketName = listing.GetProperty("item").GetProperty("market_hash_name").GetString() ?? marketHashName;
                        var priceUsd = listing.GetProperty("price").GetDouble();

                        item = new CSFloatItemDto
                        {
                            MarketHashName = marketName,
                            Price = Math.Round(priceUsd / 100 * usdToEur, 2)
                        };

                        break;
                    }
                    catch (Exception ex)
                    {
                        await _logLogic.AddLogAsync(LogBuilder.BuildLoggerDto($"Exception with API key index {currentKeyIndex} for '{marketHashName}': {ex.Message}", LogType.Error, LogPlace.Api, 1));
                        currentKeyIndex++;
                    }
                }

                if (item == null)
                {
                    await _logLogic.AddLogAsync(LogBuilder.BuildLoggerDto($"All API keys failed or invalid data for '{marketHashName}'", LogType.Warning, LogPlace.Api, 1));
                    return [];
                }

                items.Add(item);
            }

            await _logLogic.AddLogAsync(LogBuilder.BuildLoggerDto($"GetCSFloatItemPricesAsync completed successfully. Retrieved {items.Count} items.", LogType.Info, LogPlace.Api, 1));

            return items;
        }
        catch (HttpRequestException ex)
        {
            await _logLogic.AddLogAsync(LogBuilder.BuildLoggerDto("HttpRequestException in GetCSFloatItemPricesAsync: " + ex.Message, LogType.Error, LogPlace.Api, 1));

            return [];
        }
    }

    public static async Task<double> GetUsdToEur()
    {
        using var client = new HttpClient();

        var response = await client.GetAsync("https://open.er-api.com/v6/latest/USD");
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);

        if (doc.RootElement.GetProperty("result").GetString() != "success") return 0.0;

        return doc.RootElement.GetProperty("rates").GetProperty("EUR").GetDouble();
    }
}

