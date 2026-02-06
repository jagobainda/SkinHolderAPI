using System.Collections.Concurrent;
using System.Threading.Channels;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Globalization;

namespace SkinHolderAPI.Application.External;

public interface ISteamPriceQueueService
{
    string EnqueueRequest(string marketHashName, string country = "ES", int currency = 3, int appId = 730);
    SteamPriceTaskStatus? GetTaskStatus(string taskId);
}

public class SteamPriceQueueService : ISteamPriceQueueService, IDisposable
{
    private readonly Channel<SteamPriceRequest> _queue;
    private readonly ConcurrentDictionary<string, SteamPriceTaskStatus> _taskStatuses;
    private readonly ILogger<SteamPriceQueueService> _logger;
    private readonly PeriodicTimer _cleanupTimer;
    private readonly CancellationTokenSource _cleanupCts;
    private readonly Task _cleanupTask;
    private const int TaskStatusExpirationMinutes = 10;
    private const int MaxQueuedTasks = 10000; // Límite de seguridad
    private int _queuedCount = 0; // Cache del contador

    public SteamPriceQueueService(ILogger<SteamPriceQueueService> logger)
    {
        _queue = Channel.CreateUnbounded<SteamPriceRequest>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

        _taskStatuses = new ConcurrentDictionary<string, SteamPriceTaskStatus>();
        _logger = logger;
        
        _cleanupTimer = new PeriodicTimer(TimeSpan.FromMinutes(5));
        _cleanupCts = new CancellationTokenSource();
        _cleanupTask = RunCleanupAsync(_cleanupCts.Token);
    }

    public string EnqueueRequest(string marketHashName, string country = "ES", int currency = 3, int appId = 730)
    {
        // Protección contra sobrecarga
        if (_taskStatuses.Count >= MaxQueuedTasks)
        {
            _logger.LogWarning("Cola llena. Rechazando petición para {ItemName}", marketHashName);
            throw new InvalidOperationException("Queue is full. Please try again later.");
        }

        var taskId = Guid.NewGuid().ToString();
        
        _taskStatuses[taskId] = new SteamPriceTaskStatus
        {
            Status = "queued",
            MarketHashName = marketHashName,
            CreatedAt = DateTime.UtcNow
        };

        Interlocked.Increment(ref _queuedCount);

        var request = new SteamPriceRequest(taskId, marketHashName, country, currency, appId);
        _queue.Writer.TryWrite(request);
        
        _logger.LogDebug("Encolada petición {TaskId} para {ItemName}. Cola actual: {QueueCount}", 
            taskId, marketHashName, _queuedCount);
        
        return taskId;
    }

    public SteamPriceTaskStatus? GetTaskStatus(string taskId)
    {
        return _taskStatuses.TryGetValue(taskId, out var status) ? status : null;
    }

    internal ChannelReader<SteamPriceRequest> GetQueueReader() => _queue.Reader;

    internal void UpdateTaskStatus(string taskId, string status, string? result = null, string? error = null)
    {
        if (!_taskStatuses.TryGetValue(taskId, out var currentStatus)) return;

        if (currentStatus.Status == "queued" && status != "queued") Interlocked.Decrement(ref _queuedCount);

        decimal? price = null;
        
        if (!string.IsNullOrEmpty(result)) price = ExtractPriceFromJson(result);

        _taskStatuses[taskId] = currentStatus with
        {
            Status = status,
            Result = result,
            Price = price,
            Error = error,
            CompletedAt = status is "completed" or "failed" ? DateTime.UtcNow : null
        };

        _logger.LogDebug("Task {TaskId} actualizada a estado: {Status}", taskId, status);
    }

    internal int GetQueueCount() => _queuedCount;

    private async Task RunCleanupAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (await _cleanupTimer.WaitForNextTickAsync(cancellationToken))
            {
                CleanupExpiredTasks();
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Cleanup task finalizada correctamente");
        }
    }

    private void CleanupExpiredTasks()
    {
        var now = DateTime.UtcNow;
        
        var expiredKeys = _taskStatuses
            .Where(kvp => (now - kvp.Value.CreatedAt).TotalMinutes > TaskStatusExpirationMinutes)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expiredKeys)
        {
            if (_taskStatuses.TryRemove(key, out var removed))
            {
                if (removed.Status == "queued") Interlocked.Decrement(ref _queuedCount);
                
                _logger.LogDebug("Limpiado taskStatus expirado: {TaskId}", key);
            }
        }

        var stuckKeys = _taskStatuses
            .Where(kvp => kvp.Value.Status == "processing" && (now - kvp.Value.CreatedAt).TotalMinutes > 5)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in stuckKeys)
        {
            if (_taskStatuses.TryGetValue(key, out var stuck))
            {
                UpdateTaskStatus(key, "failed", error: "Task timeout - processing took too long");
                _logger.LogWarning("Task {TaskId} marcada como fallida por timeout", key);
            }
        }

        if (expiredKeys.Count > 0 || stuckKeys.Count > 0) 
            _logger.LogInformation("Limpieza: {Expired} expirados, {Stuck} stuck. Total en memoria: {Total}", expiredKeys.Count, stuckKeys.Count, _taskStatuses.Count);
    }

    private static decimal ExtractPriceFromJson(string input)
    {
        if (string.IsNullOrEmpty(input)) return -1m;

        try
        {
            using var doc = JsonDocument.Parse(input);

            if (!doc.RootElement.TryGetProperty("lowest_price", out var priceElement)) return -1m;

            var priceString = priceElement.GetString();
            if (string.IsNullOrEmpty(priceString)) return -1m;

            var digitsOnly = Regex.Replace(priceString, @"[^\d,.]", "");
            digitsOnly = digitsOnly.Replace(',', '.');

            return decimal.TryParse(digitsOnly, NumberStyles.Number, CultureInfo.InvariantCulture, out var price) ? price : -1m;
        }
        catch (JsonException)
        {
            return -1m;
        }
    }

    public void Dispose()
    {
        _cleanupCts?.Cancel();
        _cleanupTimer?.Dispose();
        
        try
        {
            _cleanupTask?.Wait(TimeSpan.FromSeconds(5));
        }
        catch (AggregateException)
        {
            // Expected during cancellation
        }
        
        _cleanupCts?.Dispose();
        GC.SuppressFinalize(this);
    }
}

internal record SteamPriceRequest(string RequestId, string MarketHashName, string Country, int Currency, int AppId);

public record SteamPriceTaskStatus
{
    public string Status { get; init; } = "queued";
    public string? MarketHashName { get; init; }
    public string? Result { get; init; }
    public decimal? Price { get; init; }
    public string? Error { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
}
