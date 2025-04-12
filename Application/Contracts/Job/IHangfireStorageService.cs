namespace Application.Contracts.Job;

public interface IHangfireStorageService
{
    string GetScheduleJobIdByArgId(string methodName, string queueName, Guid argId, int? from = null,
        int? count = null);
}