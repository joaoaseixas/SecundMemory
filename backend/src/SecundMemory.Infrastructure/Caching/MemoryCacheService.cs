using Microsoft.Extensions.Caching.Memory;
using SecundMemory.Application.Common.Abstractions;

namespace SecundMemory.Infrastructure.Caching;

/// <summary>
/// In-process only — fine for a single-instance deployment. Swap for a
/// distributed cache (Redis) if this ever runs as more than one instance.
/// </summary>
public class MemoryCacheService(IMemoryCache cache) : ICacheService
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) =>
        Task.FromResult(cache.TryGetValue(key, out T? value) ? value : default);

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        cache.Set(key, value, expiration ?? DefaultExpiration);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        cache.Remove(key);
        return Task.CompletedTask;
    }
}
