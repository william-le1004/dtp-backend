using System.Text.Json;
using Application.Contracts.Caching;
using Domain.DataModel;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Infrastructure.Services;

public class SystemSettingService(IConnectionMultiplexer redis, DtpDbContext dbContext) : ISystemSettingService
{
    private const string RedisPrefix = "SystemSetting:";
    private readonly IDatabase _redis = redis.GetDatabase();

    public async Task LoadAllSettingsToCacheAsync()
    {
        var settings = await dbContext.SystemSetting.ToListAsync();
        foreach (var setting in settings)
        {
            string key = $"{RedisPrefix}{setting.SettingKey}";
            string json = JsonSerializer.Serialize(setting);
            await _redis.StringSetAsync(key, json);
        }
    }

    public async Task<SystemSetting?> GetSettingAsync(SettingKey key)
    {
        string redisKey = $"{RedisPrefix}{key}";
        var cached = await _redis.StringGetAsync(redisKey);
        if (cached.HasValue)
        {
            return JsonSerializer.Deserialize<SystemSetting>(cached!);
        }

        // Fallback to DB if not in Redis
        var setting = await dbContext.SystemSetting.FirstOrDefaultAsync(s => s.SettingKey == key);
        if (setting != null)
        {
            await _redis.StringSetAsync(redisKey, JsonSerializer.Serialize(setting));
        }

        return setting;
    }

    public async Task UpdateSettingAsync(SystemSetting setting)
    {
        

        string key = $"{RedisPrefix}{setting.SettingKey}";
        await _redis.StringSetAsync(key, JsonSerializer.Serialize(setting));
    }
}
