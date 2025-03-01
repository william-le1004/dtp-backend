using System.Text.Json;
using Application.Contracts.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Services;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDistributedCache _redisCache;

    public RedisCacheService(IDistributedCache redisCache)
    {
        _redisCache = redisCache;
    }

    public async Task<T?> GetDataAsync<T>(string key) where T : class
    {
        var data = await _redisCache.GetStringAsync(key);
        if (string.IsNullOrEmpty(data)) return null;

        return JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetDataAsync<T>(string key, T data, TimeSpan? expiration = null) where T : class
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
        };
        
        await _redisCache.SetStringAsync(key, JsonSerializer.Serialize(data), options);
    }
    
    public async Task RemoveDataAsync(string key)
    {
        await _redisCache.RemoveAsync(key);
    }
}