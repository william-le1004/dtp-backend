using Domain.DataModel;

namespace Application.Contracts.Caching;
public interface ISystemSettingService
{
    Task LoadAllSettingsToCacheAsync();
    Task<SystemSetting?> GetSettingAsync(SettingKey key);
    Task UpdateSettingAsync(SystemSetting setting);
}
