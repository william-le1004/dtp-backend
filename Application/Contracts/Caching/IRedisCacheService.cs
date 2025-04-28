namespace Application.Contracts.Caching;

public interface IRedisCacheService
{
    Task<T?> GetDataAsync<T>(string key) where T : class;
    Task SetDataAsync<T>(string key, T data, TimeSpan? expiration = null) where T : class;
    Task<bool> RemoveDataAsync(string key);
}