using SkinHolderAPI.DTOs.ApiQuery;
using System.Text.Json;

namespace SkinHolderAPI.DataService.Apis;

public interface IApiQueryDataService
{
    Task<List<GamerPayItemDto>> GetGamerPayItemPricesAsync(List<string> itemNames);
    Task<List<CSFloatItemDto>> GetCSFloatItemPricesAsync(List<string> marketHashNames, string[] apiKeys);
}

public class ApiQueryDataService : IApiQueryDataService
{
    public async Task<List<GamerPayItemDto>> GetGamerPayItemPricesAsync(List<string> itemNames)
    {
        var items = await FetchGamerPayDataAsync();
        return [.. items.Where(i => itemNames.Contains(i.Name))];
    }

    private static async Task<GamerPayItemDto[]> FetchGamerPayDataAsync()
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
        catch (HttpRequestException)
        {
            // TODO: Logging
        }
        catch (JsonException)
        {
            // TODO: Logging
        }
        catch (Exception)
        {
            // TODO: Logging
        }

        return [];
    }

    public async Task<List<CSFloatItemDto>> GetCSFloatItemPricesAsync(List<string> marketHashNames, string[] apiKeys)
    {
        try
        {
            var usdToEur = await GetUsdToEur();
            if (usdToEur == 0.0 || marketHashNames.Count == 0) return [];

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
                        var url = $"https://csfloat.com/api/v1/listings?limit=1&sort_by=lowest_price&market_hash_name={Uri.EscapeDataString(marketHashName)}";
                        var request = new HttpRequestMessage(HttpMethod.Get, url);
                        request.Headers.Add("Authorization", apiKeys[currentKeyIndex]);

                        var response = await client.SendAsync(request);

                        if ((int)response.StatusCode == 429)
                        {
                            currentKeyIndex++;
                            continue;
                        }

                        if (!response.IsSuccessStatusCode) return [];

                        using var stream = await response.Content.ReadAsStreamAsync();
                        using var doc = await JsonDocument.ParseAsync(stream);

                        if (!doc.RootElement.TryGetProperty("data", out var dataElement) || dataElement.GetArrayLength() == 0)
                            return [];

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
                    catch (Exception)
                    {
                        // TODO: Logging
                        currentKeyIndex++;
                    }
                }

                if (item == null) return [];

                items.Add(item);
            }

            return items;
        }
        catch (HttpRequestException)
        {
            // TODO: Logging
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

