using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace FreelancerHub.Infrastructure.Extensions;

public static class DistributedCacheExtensions
{
    public static async Task SetRecordAsync<T>(this IDistributedCache cache,
        string key,
        T value,
        TimeSpan? expiration = null,
        TimeSpan? slidingExpiration = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(1),
            SlidingExpiration = slidingExpiration ?? TimeSpan.FromMinutes(1)
        };
        var jsonData = JsonSerializer.Serialize(value);
        await cache.SetStringAsync(key, jsonData, options);
    }

    public static async Task<T?> GetRecordAsync<T>(this IDistributedCache cache,
        string key)
    {
        var jsonData = await cache.GetStringAsync(key);
        return jsonData is null ? default(T) : JsonSerializer.Deserialize<T>(jsonData);
    }
}