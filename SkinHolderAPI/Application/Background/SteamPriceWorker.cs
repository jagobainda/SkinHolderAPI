using SkinHolderAPI.Application.External;

namespace SkinHolderAPI.Application.Background;

public class SteamPriceWorker(IServiceProvider serviceProvider, ILogger<SteamPriceWorker> logger, IHttpClientFactory httpClientFactory) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<SteamPriceWorker> _logger = logger;
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("SteamAPI");
    private const string SteamBaseUrl = "https://steamcommunity.com/market/priceoverview/?country={0}&currency={1}&appid={2}&market_hash_name={3}";
    private const int MaxRequestsPerMinute = 20;
    private readonly SemaphoreSlim _rateLimiter = new(MaxRequestsPerMinute, MaxRequestsPerMinute);
    private readonly CancellationTokenSource _resetCts = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Steam Price Worker iniciado - Rate limit: {MaxRequests}/min", MaxRequestsPerMinute);

        _ = ResetRateLimiterAsync(stoppingToken);

        var queueService = _serviceProvider.GetRequiredService<SteamPriceQueueService>();
        var queueReader = queueService.GetQueueReader();

        await foreach (var request in queueReader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await _rateLimiter.WaitAsync(stoppingToken);
                
                _logger.LogDebug("Procesando petición de Steam para: {MarketHashName}", request.MarketHashName);
                
                queueService.UpdateTaskStatus(request.RequestId, "processing");
                
                var result = await FetchSteamPriceAsync(request, stoppingToken);
                
                if (string.IsNullOrEmpty(result)) queueService.UpdateTaskStatus(request.RequestId, "failed", error: "No data returned from Steam API");
                else queueService.UpdateTaskStatus(request.RequestId, "completed", result: result);
                
                _logger.LogDebug("Petición completada: {MarketHashName} - Queue size: {QueueSize}", request.MarketHashName, queueService.GetQueueCount());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando petición de Steam: {MarketHashName}", request.MarketHashName);
                queueService.UpdateTaskStatus(request.RequestId, "failed", error: ex.Message);
            }
        }
    }

    private async Task<string> FetchSteamPriceAsync(SteamPriceRequest request, CancellationToken cancellationToken)
    {
        var url = string.Format(SteamBaseUrl, request.Country, request.Currency, request.AppId, request.MarketHashName);

        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            
            if (response.IsSuccessStatusCode) return await response.Content.ReadAsStringAsync(cancellationToken);
            
            _logger.LogWarning("Steam API respondió con {StatusCode} para {Item}", response.StatusCode, request.MarketHashName);
            return string.Empty;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error HTTP llamando a Steam API para {Item}", request.MarketHashName);
            return string.Empty;
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("Timeout llamando a Steam API para {Item}", request.MarketHashName);
            return string.Empty;
        }
    }

    private async Task ResetRateLimiterAsync(CancellationToken stoppingToken)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, _resetCts.Token);
        using var resetTimer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        
        try
        {
            while (await resetTimer.WaitForNextTickAsync(linkedCts.Token))
            {
                var currentCount = _rateLimiter.CurrentCount;
                var toRelease = MaxRequestsPerMinute - currentCount;
                
                if (toRelease > 0 && toRelease <= MaxRequestsPerMinute)
                {
                    try
                    {
                        _rateLimiter.Release(toRelease);
                        _logger.LogDebug("Rate limiter reseteado: {Released} slots liberados", toRelease);
                    }
                    catch (SemaphoreFullException)
                    {
                        _logger.LogDebug("Rate limiter ya está al máximo");
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Rate limiter reset finalizado");
        }
    }

    public override void Dispose()
    {
        _resetCts?.Cancel();
        _resetCts?.Dispose();
        _rateLimiter?.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}

