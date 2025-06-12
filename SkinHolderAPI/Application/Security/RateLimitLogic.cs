using System.Collections.Concurrent;

namespace SkinHolderAPI.Application.Security;

public interface IRateLimitLogic
{
    Task<bool> IsAllowedAsync(string key, int limit);
}

public class RateLimitLogic : IRateLimitLogic
{
    private readonly ConcurrentDictionary<string, List<DateTime>> _requests = new();

    public Task<bool> IsAllowedAsync(string key, int limit)
    {
        var now = DateTime.UtcNow;
        var window = TimeSpan.FromMinutes(1);

        var timestamps = _requests.GetOrAdd(key, _ => new List<DateTime>());

        lock (timestamps)
        {
            timestamps.RemoveAll(ts => (now - ts) > window);

            if (timestamps.Count >= limit) return Task.FromResult(false);

            timestamps.Add(now);
            return Task.FromResult(true);
        }
    }
}