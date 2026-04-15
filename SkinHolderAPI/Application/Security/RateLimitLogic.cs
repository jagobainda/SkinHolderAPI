using System.Collections.Concurrent;

namespace SkinHolderAPI.Application.Security;

public interface IRateLimitLogic
{
    Task<bool> IsAllowedAsync(string key, int limit);
}

public class RateLimitLogic(ILogger<RateLimitLogic> logger) : IRateLimitLogic
{
    private class RequestCounter
    {
        public int Count;
        public DateTime PeriodStart;
    }

    private readonly ConcurrentDictionary<string, RequestCounter> _requests = new();
    private readonly ILogger<RateLimitLogic> _logger = logger;

    public Task<bool> IsAllowedAsync(string key, int limit)
    {
        var now = DateTime.UtcNow;
        var window = TimeSpan.FromMinutes(1);

        var counter = _requests.GetOrAdd(key, _ => new RequestCounter { Count = 0, PeriodStart = now });

        lock (counter)
        {
            if (now - counter.PeriodStart > window)
            {
                counter.Count = 1;
                counter.PeriodStart = now;
                return Task.FromResult(true);
            }

            if (counter.Count < limit)
            {
                counter.Count++;
                return Task.FromResult(true);
            }

            _logger.LogWarning("Rate limit alcanzado para key={Key}, limit={Limit}, count={Count}", key, limit, counter.Count);
            return Task.FromResult(false);
        }
    }
}
