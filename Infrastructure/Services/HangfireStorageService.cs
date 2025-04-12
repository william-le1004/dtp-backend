using Application.Contracts.Job;
using Hangfire;
using Hangfire.Storage;

namespace Infrastructure.Services;

public class HangfireStorageService(JobStorage jobStorage) : IHangfireStorageService
{
    private readonly IMonitoringApi _api = jobStorage.GetMonitoringApi();

    public string GetScheduleJobIdByArgId(string methodName, string queueName, Guid argId, int? from = null, int? count = null)
    {
        var list = _api.ScheduledJobs(from ?? 0, count ?? int.MaxValue);
        if (list == null) return string.Empty;
        var item = list.FirstOrDefault(kv =>
        {
            var job = kv.Value.Job;
            if (job == null) return false;
            return (Guid)(job.Args.FirstOrDefault() ?? Guid.Empty) == argId 
                   && job.Method.Name == methodName 
                   && job.Queue == queueName;
        });

        return item.Key;
    }
}