using System.Text.Json;
using Application.Contracts.Caching;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class RedisCacheService(IDistributedCache redisCache, ILogger<RedisCacheService> logger) : IRedisCacheService, IOutputCacheStore
{
    public async Task<T?> GetDataAsync<T>(string key) where T : class
    {
        var data = await redisCache.GetStringAsync(key);
        if (string.IsNullOrEmpty(data)) return null;
        if (typeof(T) == typeof(string))
        {
            return data as T;
        }
        return JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetDataAsync<T>(string key, T data, TimeSpan? expiration = null) where T : class
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(1)
        };
        string storedValue = data is string str ? str : JsonSerializer.Serialize(data);
        await redisCache.SetStringAsync(key, storedValue, options);
    }

    public async Task<bool> RemoveDataAsync(string key)
    {
        await redisCache.RemoveAsync(key);
        return true;
    }

    public ValueTask EvictByTagAsync(string tag, CancellationToken cancellationToken)
    {
        return new ValueTask(Task.CompletedTask);
    }

    public async ValueTask<byte[]?> GetAsync(string key, CancellationToken cancellationToken)
    {
        if (!key.ToLower().Contains("odata"))
            return null;
        ArgumentNullException.ThrowIfNull(key, "key");

        logger.LogInformation("Checking Cache");

        var val =await  redisCache.GetAsync(key, cancellationToken);
        if (val == null)
        {
            logger.LogWarning("Cache miss");
        }
        else
        {
            logger.LogInformation("Cache hit");
        }
        return val;
    }

    public async ValueTask SetAsync(string key, byte[] value, string[]? tags, TimeSpan validFor, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(key, "key");
        ArgumentNullException.ThrowIfNull(value, "value");


        var options = new DistributedCacheEntryOptions();
        options.SetAbsoluteExpiration(validFor);

        await redisCache.SetAsync(key, value,options, cancellationToken);
        logger.LogInformation("Cache stored in redis");
    }
}